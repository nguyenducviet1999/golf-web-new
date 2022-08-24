using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.TournamentController.Request;
using Golf.Core.Dtos.Controllers.TournamentController.Response;
using Golf.Core.Dtos.Groups;
using Golf.Core.Exceptions;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Tuanament;
using Golf.Domain.Tournaments;
using Golf.EntityFrameworkCore.Repositories;
using Golf.HttpApi.Host.Helpers;
using Golf.Services;
using Golf.Services.Tournaments;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly TournamentService _tournamentService;
        private readonly GroupService _groupService;
        private readonly TournamentRepository _tournamentRepository;

        public TournamentsController(GroupService groupService, TournamentRepository tournamentRepository, TournamentService tournamentService)
        {
            _groupService = groupService;
            _tournamentRepository = tournamentRepository;
            _tournamentService = tournamentService;
        }
        /// <summary>
        /// lọc danh sách giải đấu
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="startIndex"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("{filter}/{startIndex}")]
        public ActionResult<List<TournamentResponse>> FilterTournamment(TournamentFilter filter, int startIndex, [FromQuery] DateTime? date, [FromQuery] string searchKey)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.FilterTournamment(currentGolfer.Id, filter, date, searchKey==null?"":searchKey.Trim().ToLower(),startIndex);
        }
        /// <summary>
        /// tìm kiếm giải đấu
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Search/{searchKey}/{startIndex}")]
        public ActionResult<List<TournamentResponse>> Search(string searchKey, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.Search(currentGolfer.Id, searchKey == null ? "" : searchKey.Trim().ToLower(), startIndex);
        }
        /// <summary>
        /// Tạo đăng ký giải đấu
        /// </summary>
        /// <param name="tournamentRequest">Dữ loieeju giải đấu</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<TournamentResponse> Add([FromBody] TournamentRequest tournamentRequest)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_tournamentService.Add(currentGolfer.Id, tournamentRequest));
        }
        /// <summary>
        /// Sủa thông tin giải đấu
        /// </summary>
        /// <param name="tournamentID">Định danh giải đấu</param>
        /// <param name="tournamentRequest">Dữ liệu cập nhật</param>
        /// <returns></returns>
        [HttpPut("{tournamentID}")]
        public ActionResult<bool> Edit(Guid tournamentID, [FromBody] TournamentRequest tournamentRequest)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.Edit(currentGolfer.Id, tournamentID, tournamentRequest);
        }
        /// <summary>
        /// Hủy đăng ký giải đấu
        /// </summary>
        /// <param name="tournamentID"></param>
        /// <returns></returns>
        [HttpPut("Cancel/{tournamentID}")]
        public ActionResult<bool> Cancel(Guid tournamentID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.Cancel(currentGolfer.Id, tournamentID);
        }
        /// <summary>
        /// Xóa giải đấu
        /// </summary>
        /// <param name="tournamentID"></param>
        /// <returns></returns>
        [HttpDelete("{tournamentID}")]
        public async Task<ActionResult<bool>> Delete(Guid tournamentID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            await _tournamentService.Delete(currentGolfer.Id, tournamentID);
            return Ok(true);
        }
        /// <summary>
        /// Gửi yêu cầu tham gia
        /// </summary>
        /// <param name="tournamentID">Định danh giải đấu</param>
        /// <returns></returns>
        [HttpPost("joinRequest/{tournamentID}")]
        public ActionResult<bool> AddJoinRequest(Guid tournamentID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.AddJoindRequest(currentGolfer.Id, tournamentID);
        }
        /// <summary>
        /// Xác nhận thành viên
        /// </summary>
        /// <param name="memberID"></param>
        /// <returns></returns>
        [HttpPut("Confirm/{memberID}")]
        public ActionResult<bool> ConfirmJoindRequest(Guid memberID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.ConfirmJoindRequest(currentGolfer.Id, memberID).Result;
        } 
        /// <summary>
        /// Không chấp nhận thành viên
        /// </summary>
        /// <param name="memberID"></param>
        /// <returns></returns>
        [HttpPut("Reject/{memberID}")]
        public ActionResult<bool> RejectJoindRequest(Guid memberID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.RejectJoindRequest(currentGolfer.Id, memberID).Result;
        }
        /// <summary>
        /// Lấy các yêu cầu đã hủy
        /// </summary>
        /// <param name="tournamentID"></param>
        /// <returns></returns>
        [HttpPut("CancelRequest/{tournamentID}")]
        public ActionResult<bool> CancelJoindRequest(Guid tournamentID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.CancelJoindRequest(currentGolfer.Id, tournamentID);
        }
        /// <summary>
        /// Lấy danh sách yêu cầu tham gia nhón
        /// </summary>
        /// <param name="tournamentID">Định danh giải đấu</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns></returns>
        [HttpGet("joinRequest/{tournamentID}/{startIndex}")]
        public ActionResult<List<TournamentMemberResponse>> GetJoinRequest(Guid tournamentID, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.GetJoinRequest(currentGolfer.Id, tournamentID, startIndex);
        }
        /// <summary>
        /// Lấy danh sách thành viên giải đấu
        /// </summary>
        /// <param name="tournamentID"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{tournamentID}/member/{startIndex}")]
        public ActionResult<List<TournamentMemberResponse>> GetJoined(Guid tournamentID, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.GetMemberTournament(currentGolfer.Id, tournamentID, startIndex);
        }
        /// <summary>
        /// tìm kiếm theo loại thành viên giải đấu(  Requested, Joined, Rejected)
        /// </summary>
        /// <param name="tournamentID">định danh giải đấu</param>
        /// <param name="searchKey">Từ khóa</param>
        /// <param name="tournamentMemberStatus">Loại thành viên</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns></returns>
        [HttpGet("{tournamentID}/member/{tournamentMemberStatus}/search/{startIndex}")]
        public ActionResult<List<TournamentMemberResponse>> SearchTournamentMember(Guid tournamentID, [FromQuery] string searchKey, TournamentMemberStatus tournamentMemberStatus, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.SearchMember(currentGolfer.Id, tournamentID, searchKey == null ? "" : searchKey.Trim().ToLower(), tournamentMemberStatus, startIndex);
        }
        /// <summary>
        /// lấy thong tin chi tiết thành viên
        /// </summary>
        /// <param name="memberID"></param>
        /// <returns></returns>
        [HttpGet("member/{memberID}")]
        public ActionResult<TournamentMemberResponseDetail> GetMemberResponse(Guid memberID)
        {
            return _tournamentService.GetTournamentMemberResponseDetail(memberID);
        }
        /// <summary>
        /// Lấy danh sách yêu cầu tham gia giải đấu
        /// </summary>
        /// <param name="tournamentID"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{tournamentID}/Request/{startIndex}")]
        public ActionResult<List<TournamentMemberResponse>> GetRequested(Guid tournamentID, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _tournamentService.GetRequestedMember(currentGolfer.Id, tournamentID, startIndex);
        }
        /// <summary>
        /// API tạo nhóm giải đấu
        /// </summary>
        /// <param name="tournamentID"></param>
        /// <returns></returns>
        [HttpPut("{tournamentID}/Group")]
        public async Task<ActionResult<GroupResponse>> AddTournamentGroup(Guid tournamentID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Not found tournament!");
            }
            if (tournament.Status == TournamentStatus.Requested)
            {
                throw new BadRequestException("Can not add tournament group!");
            }
            if (tournament.GroupID != null && tournament.GroupID != Guid.Empty)
            {
                throw new BadRequestException("Group already exists");
            }
            else
            {
                return await _groupService.AddTournamentGroup(currentGolfer.Id,tournamentID);
            }
        }
    }
}