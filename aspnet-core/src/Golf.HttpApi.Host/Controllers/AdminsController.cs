using AutoMapper;
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.AdminController.Account.Request;
using Golf.Core.Dtos.Controllers.AdminController.Tournament;
using Golf.Core.Dtos.Controllers.TournamentController.Response;
using Golf.Core.Exceptions;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Golfer.UserSetting;
using Golf.Domain.Shared.System;
using Golf.Domain.Shared.Tuanament;
using Golf.Domain.Tournaments;
using Golf.EntityFrameworkCore.Repositories;
using Golf.HttpApi.Host.Helpers;
using Golf.Services.AdminService;
using Golf.Services.Courses;
using Golf.Services.Notifications;
using Golf.Services.Tournaments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(roles: "System Admin")]
    public class AdminsController : ControllerBase
    {
        private readonly TournamentRepository _tournamentRepository;
        private readonly TournamentService _tournamentService;
        private readonly NotificationService _notificationService;
        private readonly CourseManagerService _courseManagerService;
        private readonly SystemSettingRepository _systemSettingRepository;
        private readonly GolferRepository _golferRepository;
        private readonly MemberShipService _memberShipService;
        private readonly AccountManageService _accountManageService;
        private readonly IMapper _mapper;
        public AdminsController(NotificationService notificationService, AccountManageService accountManageService, IMapper mapper, MemberShipService memberShipService, TournamentService tournamentService, GolferRepository golferRepository, SystemSettingRepository systemSettingRepository, CourseManagerService courseManagerService, TournamentRepository tournamentRepository)
        {
            _notificationService = notificationService;
            _accountManageService = accountManageService;
            _mapper = mapper;
            _memberShipService = memberShipService;
            _tournamentService = tournamentService;
            _golferRepository = golferRepository;
            _tournamentRepository = tournamentRepository;
            _courseManagerService = courseManagerService;
            _systemSettingRepository = systemSettingRepository;
        }
        //tournament mananger
        /// <summary>
        /// Xác nhận giải dấu 
        /// </summary>
        /// <param name="tournamentID">Định danh giải đấu</param>
        /// <returns>Kết quả</returns>
        [HttpPut("tournaments/{tournamentID}/Confirm")]
        public async Task<ActionResult<bool>> ConfirmTournament(Guid tournamentID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Not found tournament");
            }
            if (tournament.Status == TournamentStatus.Requested)
            {
                tournament.Status = TournamentStatus.Registered;
                _tournamentRepository.UpdateEntity(tournament);
                await _notificationService.NotificationConfirmTournament(golfer.Id,tournament.OwnerID,tournamentID);
                return true;
            }
            else throw new BadRequestException("Can not confirm");
        }
        /// <summary>
        /// Lấy danh sách giải đấu
        /// </summary>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns>Danh sách giải đấu</returns>
        [HttpGet("tournaments/request/{startIndex}")]
        public ActionResult<List<TournamentAdminResponse>> GetTournamentRequest(int startIndex)
        {
            List<TournamentAdminResponse> tournamentAdminResponses = new List<TournamentAdminResponse>();
            var tournaments = _tournamentRepository.Find(t => t.Status == TournamentStatus.Requested)
                                                    .OrderBy(t => t.DateTime).Skip(startIndex)
                                                    .Take(10).ToList();
            foreach (var i in tournaments)
            {
                var tournamentAdminResponse = _mapper.Map<TournamentAdminResponse>(i);
                tournamentAdminResponses.Add(tournamentAdminResponse);
            }
            return tournamentAdminResponses;
        }
        /// <summary>
        /// Từ chối giải đấu 
        /// </summary>
        /// <param name="tournamentID"> Định danh giải đấu</param>
        /// <returns>Kết quả từ chối</returns>
        [HttpPut("tournaments/{tournamentID}/Reject")]
        public async Task<ActionResult<bool>> RejectTournament(Guid tournamentID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Not found tournament");
            }
            if (tournament.Status == TournamentStatus.Requested)
            {
                tournament.Status = TournamentStatus.Rejected;
                _tournamentRepository.UpdateEntity(tournament);
               await _notificationService.NotificationRejectTournament(golfer.Id, tournament.OwnerID, tournamentID);
                return true;
            }
            else throw new BadRequestException("Can not confirm");
        }
        /// <summary>
        /// Xóa giải đấu
        /// </summary>
        /// <param name="tournamentID">Định danh giải đấu</param>
        /// <returns>Kết quả</returns>
        [HttpPut("tournaments/{tournamentID}/Delete")]
        public ActionResult<bool> DeleteTournament(Guid tournamentID)
        {
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Not found tournament");
            }
            _tournamentRepository.RemoveEntity(tournament);
            return true;

        }
        //course mananger
        /// <summary>
        /// Lấy danh sách sân yêu cầu
        /// </summary>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns></returns>
        [HttpGet("Courses/request/{startIndex}")]
        public ActionResult<List<Course>> GetCourseRequests(int startIndex)
        {
            return _courseManagerService.GetCourseRequests(startIndex);

        }
        /// <summary>
        /// Lấy danh sách địa điểm yêu cầu
        /// </summary>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns></returns>
        [HttpGet("location/request/{startIndex}")]
        public ActionResult<List<Location>> GetLocatinRequests(int startIndex)
        {
            return _courseManagerService.GetLocationRequests(startIndex);

        }
        /// <summary>
        /// Xác nhận sân yêu cầu
        /// </summary>
        /// <param name="courseID">Dịnh danh sân</param>
        /// <returns></returns>
        [HttpPut("Courses/{courseID}/confirm")]
        public async Task<ActionResult<bool>> ConfirmCourse(Guid courseID)
        {
            return Ok(await _courseManagerService.ConfirmCourseRequest(courseID));
        }
        /// <summary>
        /// Xác nhân địa điểm yêu cầu. (xác nhận tất cả các sân của địa điểm)
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        [HttpPut("location/{locationID}/confirm")]
        public ActionResult<bool> ConfirmLocatin(Guid locationID)
        {
            return _courseManagerService.ConfirmLocationRequest(locationID);
        }
        //Account mananger
        [HttpPost("Account/CourseAdmin")]
        public async Task<ActionResult<Golfer>> AddCourseAdminAccount([FromForm]AddCourseAdminRequest addCourseAdminRequest)
        {
            return Ok(await _accountManageService.AddCourseAdminAccount(addCourseAdminRequest)) ;
        } 
        [HttpPost("Account/LocationAdmin")]
        public async Task<ActionResult<Golfer>> AddLocationAdminAccount([FromForm]AddLocationAdminRequest addCourseAdminRequest)
        {
            return Ok(await _accountManageService.AddLocationAdminAccount(addCourseAdminRequest)) ;
        }
        [HttpPut("Account/SetCourseAdmin/{golferID}")]
        public async Task<ActionResult<Golfer>> SetCourseAdmin(Guid golferID,[FromForm]List<Guid> courseIDs)
        {
            return Ok(await _accountManageService.SetCourseAdmin(golferID, courseIDs)) ;
        }
        //membership
        //[HttpPost("membership/AddMemberShip/{golferID}")]
        //public ActionResult<Boolean> AddCourseMemberShip(Guid golferID)
        //{
        //    var golfer = _golferRepository.Get(golferID);
        //    var currentGolfer = (Golfer)HttpContext.Items["Golfer"];

        //    if (golfer == null)
        //        throw new NotFoundException("Golfer isn't exit!");

        //    return Ok(_memberShipService.AddMemberShip(golferID));
        //}
        //[HttpGet("membership/Requests/{startIndex}")]
        //public ActionResult<MinimizedGolfer> GetMemberShipRequest(int startIndex)
        //{
        //    return Ok(_memberShipService.GetMemberShipRequest(startIndex));
        //}
        //[HttpPut("membership/ConfirmMemberShip/{golferID}")]
        //public ActionResult<MinimizedGolfer> ConfirmMemberShipRequest(Guid golferID)
        //{
        //    return Ok(_memberShipService.ConfirmMemberShip(golferID));
        //}
        //setting
        [HttpGet("Setting")]
        public ActionResult<SystemSetting> GetSetting()
        {
            var sysSetting = _systemSettingRepository.FirstOrDefault();

            if (sysSetting != null)
            {
                return sysSetting;
            }
            else
            {
                SystemSetting systemSetting = new SystemSetting();
                _systemSettingRepository.Add(systemSetting);
                return systemSetting;
            }
        }

        [HttpPut("Setting")]
        public ActionResult<bool> UpdateSetting(Setting setting)
        {
            var sysSetting = _systemSettingRepository.FirstOrDefault();
            if (sysSetting == null)
            {
                var sysSt = new SystemSetting();
                sysSt.Setting = setting;
                _systemSettingRepository.Add(sysSt);
                return true;
            }
            else
            {
                sysSetting.Setting = setting;
                _systemSettingRepository.UpdateEntity(sysSetting);
                return true;
            }

        }

        [HttpPut("UpdateUSetting")]
        public ActionResult<bool> UpdateUSetting()
        {
            var uses = _golferRepository.GetAll().ToList();
            foreach (var i in uses)
            {
                i.Setting = new UserSetting();
                _golferRepository.UpdateEntity(i);
            }
            return true;

        }  
    }
}
