using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Golf.HttpApi.Host.Helpers;
using Golf.Domain.Shared.Resources;
using Golf.Services;
using Golf.Core.Common.Golfer;

using Golf.Core.Dtos.Controllers.ProfileController.Requests;
using Golf.Core.Dtos.Controllers.ProfileController.Responses;
using Golf.Domain.GolferData;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        // PUT: api/Profile/PersonalInformation
        /// <summary>
        /// Sửa thông tin cá nhân người dùng trong profile
        /// </summary>
        /// <param name="request">Dữ liệu chỉnh sửa</param>
        /// <returns></returns>
        [HttpPut("PersonalInformation")]
        public async Task<ActionResult<FullProfileResponse>> EditPersonalInformation(EditPersonalInformationRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var fullProfile = await _profileService.EditPersonalInformation(currentGolfer, request);
            return Ok(fullProfile);
        }

        // PUT: api/Profile/ContactInformation
        /// <summary>
        /// Sửa thông tin liên hệ của người dùng
        /// </summary>
        /// <param name="request">Dữ liệu chỉnh sửa</param>
        /// <returns></returns>
        [HttpPut("ContactInformation")]
        public async Task<ActionResult<FullProfileResponse>> EditContactInformation(EditContactInformationRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var fullProfile = await _profileService.EditContactInformation(currentGolfer, request);
            return Ok(fullProfile);
        }

        // PUT: api/Profile/OccupationInformation
        /// <summary>
        /// Sửa thông tin công việc người dùng 
        /// </summary>
        /// <param name="request">Dữ liệu cập nhật</param>
        /// <returns></returns>
        [HttpPut("OccupationInformation")]
        public ActionResult<FullProfileResponse> EditOccupationInformation(EditOccupationInformation request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var fullProfile = _profileService.EditOccupationInformation(currentGolfer, request);
            return Ok(fullProfile);
        }

        // PUT: api/Profile/ClothingInformation
        /// <summary>
        /// Sửa thông tin trang phục người dùng
        /// </summary>
        /// <param name="request">Dữ liệu chỉnh sửa</param>
        /// <returns></returns>
        [HttpPut("ClothingInformation")]
        public ActionResult<FullProfileResponse> EditClothingInformation(EditClothingInformationRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var fullProfile = _profileService.EditClothingInformation(currentGolfer, request);
            return Ok(fullProfile);
        }

        // PUT: api/Profile/AddressInformation
        /// <summary>
        /// Sửa dữ liệu thông tin vị trí người chơi
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("AddressInformation")]
        public ActionResult<FullProfileResponse> EditAddressInformation(EditAddressInformationRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var fullProfile = _profileService.EditAddressInformation(currentGolfer, request);
            return Ok(fullProfile);
        }

        // PUT: api/Profile/Quote
        [HttpPut("Quote")]
        public ActionResult<FullProfileResponse> EditQuote(EditQuoteRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var fullProfile = _profileService.EditQuote(currentGolfer, request);
            return Ok(fullProfile);
        }

        // PUT: api/Profile/Avatar
        /// <summary>
        /// Sửa ảnh đại diện
        /// </summary>
        /// <param name="request">Tệp dữ liệu ảnh </param>
        /// <returns></returns>
        [HttpPut("Avatar")]
        public async Task<ActionResult> SetAvatar([FromForm] SetAvatarRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            currentGolfer = await _profileService.SetAvatar(currentGolfer, request);
            return Ok(currentGolfer);
        }

        // PUT: api/Profile/Cover
        /// <summary>
        /// Cập nhật ảnh bìa người dùng
        /// </summary>
        /// <param name="request">Tệp file ảnh</param>
        /// <returns></returns>
        [HttpPut("Cover")]
        public async Task<ActionResult> SetCover([FromForm] SetCoverRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            currentGolfer = await _profileService.SetCover(currentGolfer, request);
            return Ok(currentGolfer);
        }

        /// <summary>
        /// Đổi mật khẩu 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // PUT: api/Profile/Password
        [HttpPut("Password")]
        public async Task<ActionResult<bool>> ChangePassword(ChangePasswordRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            await _profileService.ChangePassword(currentGolfer, request);
            return Ok(true);
        }
       
    }
}
