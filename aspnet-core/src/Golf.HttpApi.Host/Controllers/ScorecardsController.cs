using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

using Golf.Services;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Post;
using Golf.HttpApi.Host.Helpers;

using Golf.Core.Common.Post;
using Golf.Core.Common.Golfer;

using Golf.Core.Dtos.Controllers.ScorecardController.Responses;
using Golf.Core.Dtos.Controllers.GolfersController.Responses;
using Golf.Core.Dtos.Controllers.ScorecardController.Requests;
using Golf.Domain.Scorecard;
using Golf.Domain.Shared.Scorecard;
using Golf.EntityFrameworkCore.Repositories;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ScorecardsController : ControllerBase
    {
        private readonly ScorecardService _scorecardService;
        private readonly ScorecardRepository _scorecardRepository;
        private readonly GolferService _golferService;

        public ScorecardsController(ScorecardRepository scorecardRepository, ScorecardService scorecardService, GolferService golferService)
        {
            _scorecardRepository = scorecardRepository;
            _scorecardService = scorecardService;
            _golferService = golferService;
        }

        // GET: api/Scorecards/{ScorecardID}/Detail
        /// <summary>
        /// Lấy dữ liệu bảng điểm
        /// </summary>
        /// <param name="ScorecardID">Định danh bảng điểm</param>
        /// <returns></returns>
        [HttpGet("{ScorecardID}")]
        public ActionResult<GetScorecardDetailResponse> GetScorecardDetail(Guid ScorecardID)
        {
            GetScorecardDetailResponse scorecardDetail = _scorecardService.GetScorecardDetail(ScorecardID);
            return Ok(scorecardDetail);
        }
        /// <summary>
        /// Lấy danh sách bảng điểm chi tiết bằng danh sách định danh 
        /// </summary>
        /// <param name="scorecardIDs"></param>
        /// <returns></returns>
        [HttpPost("ListScorecard")]
        public ActionResult<List<GetScorecardDetailResponse>> GetScorecardsDetail([FromForm] List<Guid> scorecardIDs)
        {
            List<GetScorecardDetailResponse> getScorecardDetailResponses = new List<GetScorecardDetailResponse>();
            foreach (var i in scorecardIDs)
            {
                GetScorecardDetailResponse scorecardDetail = _scorecardService.GetScorecardDetail(i);
                if (scorecardDetail != null)
                {
                    getScorecardDetailResponses.Add(scorecardDetail);
                }
            }

            return Ok(getScorecardDetailResponses);
        }
        /// <summary>
        /// Xóa scorecard
        /// </summary>
        /// <param name="ScorecardID">Dịnh danh bảng điểm</param>
        /// <returns></returns>
        [HttpDelete("{ScorecardID}")]
        public ActionResult<GetScorecardDetailResponse> DeletePendingScorecard(Guid ScorecardID)
        {
            var result = _scorecardService.DeletePendingScoreCard(ScorecardID);
            return Ok(result);
        }

        /// <summary>
        /// Lưu bảng điểm 
        /// </summary>
        /// <param name="request">Dữ liệu bảng điểm</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<bool> PostScorecard(SaveScorecardRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var Golfer = _golferService.GetGolfer(currentGolfer.Id);
            if (Golfer == null)
                return NotFound("Golfer is't exit");
            request.Type = ScorecardType.Practice;
            Scorecard scorecard = _scorecardService.SaveScorecard(Golfer, request);
            return Ok(scorecard != null ? true : false);
        }
    }
}