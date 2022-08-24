using Golf.Core.Dtos.Controllers.AdminController.Account.Request;
using Golf.Core.Exceptions;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Golfer;
using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.OdooAPI;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.AdminService
{
    public class AccountManageService
    {
        private readonly UserManager<Golfer> _golferManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ProfileRepository _profileRepository;
        private readonly SignInManager<Golfer> _signInManager;
        private const string GoogleApiTokenInfoUrl = "https://oauth2.googleapis.com/tokeninfo?id_token={0}";
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly OdooAPIService _odooAPIService;
        private readonly CourseRepository _courseRepository;
        private readonly LocationRepository _locationRepository;

        public AccountManageService(
            LocationRepository locationRepository,
            CourseRepository courseRepository,
            OdooAPIService odooAPIService,
            UserManager<Golfer> golferManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ProfileRepository profileRepository,
            SignInManager<Golfer> signInManager,
            DatabaseTransaction databaseTransaction)
        {
            _courseRepository = courseRepository;
            _locationRepository = locationRepository;
            _odooAPIService = odooAPIService;
            _golferManager = golferManager;
            _roleManager = roleManager;
            _profileRepository = profileRepository;
            _signInManager = signInManager;
            _databaseTransaction = databaseTransaction;
        }
        //Account mananger
        public async Task<Golfer> AddCourseAdminAccount(AddCourseAdminRequest request)
        {
            _databaseTransaction.BeginTransaction();
            try
            {
                var courses = _courseRepository.Find(c => request.CourseIDs.Contains(c.ID)).ToList();
                if (courses.Count() <= 0)
                {
                    throw new NotFoundException("Not found course");
                }
                var newGolfer = new Golfer()
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = request.PhoneNumber,
                    UserName = request.Email,
                    Email = request.Email,
                    NormalizedUserName = request.PhoneNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Avatar = "",
                    Cover = "",
                    Handicap = 0.0,
                    StartHandicap = 0.0,
                    IDX = 0.0
                };
                var tmp = await _golferManager.CreateAsync(newGolfer, request.Password);
                if (tmp.Succeeded)
                {
                    await _golferManager.AddToRolesAsync(newGolfer, new List<string>() { RoleNormalizedName.CourseAdmin });
                    _profileRepository.Add(new Profile
                    {
                        ID = newGolfer.Id,
                        CountryID = request.CountryID,
                        ShirtSize = -1,
                        ShoesSize = -1,
                        PantsSize = -1

                    });
                    foreach (var i in courses)
                    {
                        i.OwnerID = newGolfer.Id;
                        _courseRepository.SafeUpdate(i);
                    }
                    await _databaseTransaction.Commit();
                    return newGolfer;
                }
                else
                {
                    throw new Exception(tmp.Errors.ToString());
                }
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception("Create Account Error: " + exception.Message);
            }
        }
        public async Task<Golfer> AddLocationAdminAccount(AddLocationAdminRequest request)
        {
            var courses = _courseRepository.Find(c => request.LocationIDs.Contains(c.LocationID)).ToList();
            if (courses.Count() <= 0)
            {
                throw new NotFoundException("Not found course");
            }
            AddCourseAdminRequest addCourseAdminRequest = new AddCourseAdminRequest()
            {
                Email = request.Email,
                CourseIDs = courses.Select(c => c.ID).ToList(),
                City = request.City,
                CountryID = request.CountryID,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                Street = request.Street,
                Zip = request.Zip
            };
               return await this.AddCourseAdminAccount(addCourseAdminRequest);
        }
        public async Task<bool> SetCourseAdmin(Guid adminID, List<Guid> courseIDs)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var courses = _courseRepository.Find(c => courseIDs.Contains(c.LocationID)).ToList();
                if (courses.Count() <= 0)
                {
                    throw new NotFoundException("Not found course");
                }
                var admin = await _golferManager.FindByIdAsync(adminID.ToString());
                if (admin == null)
                {
                    throw new NotFoundException("Not found course admin");
                }
                if ((await _golferManager.GetRolesAsync(admin)).Contains(RoleName.CourseAdmin) == false)
                {
                    throw new ForbiddenException("Unauthorized");
                }
                foreach (var i in courses)
                {
                    i.OwnerID = adminID;
                    _courseRepository.SafeUpdate(i);
                }
                await _databaseTransaction.Commit();
                return true;
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception(exception.Message);
            }

        }

    }
}
