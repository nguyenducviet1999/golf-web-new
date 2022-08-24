using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;

using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Resources;
using Golf.Domain.Shared.Resources;

using Golf.Core.Common.Golfer;
using Golf.Core.Exceptions;
using Golf.Core.Dtos.Controllers.ProfileController.Requests;
using Golf.Core.Dtos.Controllers.ProfileController.Responses;
using Golf.Domain.GolferData;
using Golf.Services.OdooAPI;
using Golf.Domain.Shared.OdooAPI;
using Golf.Core.Common.Odoo.OdooResponse;
using Golf.Services.Resource;

namespace Golf.Services
{
    public class ProfileService
    {
        private readonly PhotoService _photoService;
        private readonly ProfileRepository _profileRepository;
        private readonly PhotoRepository _photoRepository;
        private readonly UserManager<Golfer> _golferManager;
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly OdooAPIService _odooAPIService;
        private readonly ResourceService _resourceService;

        public ProfileService(
            ResourceService resourceService,
            OdooAPIService odooAPIService,
            ProfileRepository golferprofileRepository,
            UserManager<Golfer> golferManager,
            PhotoService phototService,
            PhotoRepository photoRepository,
            DatabaseTransaction databaseTransaction)
        {
            _resourceService = resourceService;
            _odooAPIService = odooAPIService;
            _photoRepository = photoRepository;
            _golferManager = golferManager;
            _profileRepository = golferprofileRepository;
            _photoService = phototService;
            _databaseTransaction = databaseTransaction;
        }

        /// <summary>
        /// Lấy thông tin người dùng đầy đủ
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="golfer"></param>
        /// <returns></returns>
        private FullProfileResponse GetFullProfile(Profile profile, Golfer golfer)
        {

            return new FullProfileResponse
            {
                ID = golfer.Id,
                FirstName = golfer.FirstName,
                LastName = golfer.LastName,
                Gender = profile.Gender,
                MaritalStatus = profile.MaritalStatus,
                Birthday = profile.Birthday,
                Address = profile.Address,
                PhoneNumber = golfer.PhoneNumber,
                Email = golfer.Email,
                Website = profile.Website,
                Workfield = profile.Workfield,
                Position = profile.Position,
                Country =profile.CountryID==null?"": _resourceService.GetOdooCountrieByID((int)profile.CountryID).Result.Name,
                State = profile.StateID == null ? "" : _resourceService.GetOdooStateByID((int)profile.StateID).Result.Name,
                Company = profile.Company,
                Workplace = profile.Workplace,
                ClothesSizeType = profile.ClothesSizeType,
                ShoesSizeType = profile.ShoesSizeType,
                ShirtSize = profile.ShirtSize,
                ShoesSize = profile.ShoesSize,
                PantsSize = profile.PantsSize,
                //FootLength = profile.FootLength,
                PreferredHand = profile.PreferredHand,
                Quote = profile.Quote
            };
        }

        public FullProfileResponse GetProfile(Golfer golfer)
        {
            var profile = _profileRepository.Find(q => q.ID == golfer.Id).FirstOrDefault();
            return GetFullProfile(profile, golfer);
        }

        public async Task<FullProfileResponse> EditPersonalInformation(Golfer currentGolfer, EditPersonalInformationRequest request)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var profile = _profileRepository.Find(q => q.ID == currentGolfer.Id).FirstOrDefault();
                profile.Birthday = request.Birthday;// != null ? request.Birthday : profile.Birthday;
                profile.Gender = request.Gender;
                profile.MaritalStatus = request.MaritalStatus;
                currentGolfer.FirstName = request.FirstName;
                currentGolfer.LastName = request.LastName;
                //odoo update
                OdooProfileRequest odooProfileRequest = new OdooProfileRequest();
                odooProfileRequest.FirstName = request.FirstName;
                odooProfileRequest.LastName = request.LastName;
                odooProfileRequest.Email = currentGolfer.Email;
                var respone= await _odooAPIService.CallAPI<OdooProfileDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUpdateProfileUrl(), odooProfileRequest.getOdooProfileDto());
                if(respone.Result.Code!=200)
                {
                    throw new Exception(respone.Result.Data.Message);
                }    
                //odoo update
                await _golferManager.UpdateAsync(currentGolfer);
                _profileRepository.SafeUpdate(profile);
                await _databaseTransaction.Commit();
                return GetFullProfile(profile, currentGolfer);
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception($"Edit personal information error + {e}");
            }
        }

        public async Task<FullProfileResponse> EditContactInformation(Golfer currentGolfer, EditContactInformationRequest request)
        {
            if (currentGolfer.PhoneNumber != request.PhoneNumber && request.PhoneNumber != "")
            {
                var golferWithRequestPhoneNumber = await _golferManager.FindByNameAsync(request.PhoneNumber);
                if (golferWithRequestPhoneNumber != null)
                {
                    throw new BadRequestException("Number already in use");
                }
                else
                {
                    currentGolfer.UserName = request.PhoneNumber;
                    currentGolfer.NormalizedUserName = request.PhoneNumber;
                    currentGolfer.PhoneNumber = request.PhoneNumber;
                }
            }
            if (currentGolfer.Email != null)
            {
                if (currentGolfer.Email != request.Email && request.Email != "")
                {
                    var golferWithRequestEmail = await _golferManager.FindByEmailAsync(request.Email);
                    if (golferWithRequestEmail != null)
                    {
                        throw new BadRequestException("Email already in use");
                    }
                    else
                    {
                        currentGolfer.Email = request.Email;
                        currentGolfer.NormalizedEmail = request.Email;
                    }
                }
            }
            else
            {
                if (request.Email != "")
                {
                    var golferWithRequestEmail = await _golferManager.FindByEmailAsync(request.Email);
                    if (golferWithRequestEmail != null)
                    {
                        throw new BadRequestException("Email already in use");
                    }
                    else
                    {
                        currentGolfer.Email = request.Email;
                        currentGolfer.NormalizedEmail = request.Email;
                    }
                }
            }
            var result = await _golferManager.UpdateAsync(currentGolfer);
            if (result.Succeeded)
            {
                var profile = _profileRepository.Find(q => q.ID == currentGolfer.Id).FirstOrDefault();
                return GetFullProfile(profile, currentGolfer);
            }
            else
            {
                {
                    var errors = "";
                    foreach (var error in result.Errors)
                    {
                        errors += error.Description + " ";
                    }
                    throw new Exception(errors);
                }
            }
        }

        public FullProfileResponse EditAddressInformation(Golfer currentGolfer, EditAddressInformationRequest request)
        {
            var profile = _profileRepository.Find(q => q.ID == currentGolfer.Id).FirstOrDefault();
            profile.Address = request.Address;
            profile.CountryID = request.CountryID;
            profile.StateID = request.StateID;
            _profileRepository.UpdateEntity(profile);
            return GetFullProfile(profile, currentGolfer);
        }

        public FullProfileResponse EditOccupationInformation(Golfer currentGolfer, EditOccupationInformation request)
        {
            var profile = _profileRepository.Find(q => q.ID == currentGolfer.Id).FirstOrDefault();
            profile.Company = request.Company;
            profile.Workfield = request.Workfield;
            profile.Workplace = request.Workplace;
            profile.Website = request.Website;
            profile.Position = request.Position;
            _profileRepository.UpdateEntity(profile);
            return GetFullProfile(profile, currentGolfer);
        }

        public FullProfileResponse EditQuote(Golfer currentGolfer, EditQuoteRequest request)
        {
            var profile = _profileRepository.Find(q => q.ID == currentGolfer.Id).FirstOrDefault();
            profile.Quote = request.Quote;
            _profileRepository.UpdateEntity(profile);
            return GetFullProfile(profile, currentGolfer);
        }

        public FullProfileResponse EditClothingInformation(Golfer currentGolfer, EditClothingInformationRequest request)
        {
            var profile = _profileRepository.Find(q => q.ID == currentGolfer.Id).FirstOrDefault();
            profile.ClothesSizeType = request.ClothesSizeType;
            profile.ShoesSizeType = request.ShoesSizeType;
            profile.ShirtSize = request.ShirtSize;
            profile.PantsSize = request.PantsSize;
            profile.ShoesSize = request.ShoesSize;
            //profile.FootLength = request.FootLength;
            profile.PreferredHand = request.PreferredHand;
            _profileRepository.UpdateEntity(profile);
            return GetFullProfile(profile, currentGolfer);
        }

        public async Task<Golfer> SetAvatar(Golfer currentGolfer, SetAvatarRequest request)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var photo = await _photoService.SafeSavePhoto(currentGolfer.Id, request.File, PhotoType.Avatar);
                currentGolfer.Avatar = $"{photo.Name}";
                await _golferManager.UpdateAsync(currentGolfer);
                await _databaseTransaction.Commit();
                return currentGolfer;
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception($"Set avatar error: {exception}");
            }
        }

        public async Task<Golfer> SetCover(Golfer currentGolfer, SetCoverRequest request)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var photo = await _photoService.SafeSavePhoto(currentGolfer.Id, request.File, PhotoType.Cover);
                currentGolfer.Cover = $"{photo.Name}";
                await _golferManager.UpdateAsync(currentGolfer);
                await _databaseTransaction.Commit();
                return currentGolfer;
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception($"Set cover error: {exception}");
            }
        }

        public async Task<Golfer> ChangePassword(Golfer currentGolfer, ChangePasswordRequest request)
        {
            //odoo update
            var changePasswordDto = request.GetChangePasswordDto();
            changePasswordDto.Login = currentGolfer.Email;
            var respone = await _odooAPIService.CallAPI<ChangePasswordDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUpdateProfileUrl(), changePasswordDto);
            if (respone.Result.Code != 200)
            {
                throw new Exception(respone.Result.Data.Message);
            }
            //odoo update
            return currentGolfer;
        } 
       
        //uPdate Profile odđo
        //public bool UpdateOdooProfile()
        //{

        //}
    }
}