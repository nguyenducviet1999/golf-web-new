using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Golf.HttpApi.Host.Helpers;
using Golf.Services;
using Golf.Domain.SocialNetwork;
using Golf.Domain.GolferData;

using Golf.Core.Common.Charts;
using Golf.Core.Common.Scorecard;
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.GolfersController.Requests;
using Golf.Core.Dtos.Controllers.GolfersController.Responses;
using Golf.Core.Dtos.Controllers.ProfileController.Responses;
using Golf.Services.Courses;
using Golf.Core.Exceptions;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Shared.Golfer;
using Golf.Domain.Shared.Scorecard;
using Golf.Domain.Shared.Golfer.UserSetting;
using Golf.Core.Dtos.Controllers.GolfersController.Requests.Setting;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GolfersController : ControllerBase
    {
        private readonly GolferService _golferService;
        private readonly PhotoService _photoService;
        private readonly RelationshipService _relationshipService;
        private readonly StatisticService _statisticService;
        private readonly ScorecardService _scorecardService;
        private readonly ProfileService _profileService;
        private readonly ChartsService _chartService;
        private readonly GroupService _groupService;
        private readonly MemberShipService _courseMemberShipService;
        private readonly CourseRepository _courseRepository;
        private readonly HandicapService _handicapService;

        public GolfersController(
            HandicapService handicapService,
            CourseRepository courseRepository,
            MemberShipService courseMemberShipService,
            GolferService golferService,
            RelationshipService relationshipService,
            ScorecardService scorecardService,
            StatisticService statisticService,
            ProfileService profileService,
            ChartsService chartsService,
            GroupService groupService,
            PhotoService photoService)
        {
            _handicapService = handicapService;
            _courseRepository = courseRepository;
            _courseMemberShipService = courseMemberShipService;
            _relationshipService = relationshipService;
            _statisticService = statisticService;
            _scorecardService = scorecardService;
            _profileService = profileService;
            _golferService = golferService;
            _chartService = chartsService;
            _groupService = groupService;
            _photoService = photoService;

        }

        /// <summary>
        /// Lấy thông tin golfer
        /// </summary>
        /// <param name="GolferID">ID golfer</param>
        /// <returns></returns>
        [HttpGet("{GolferID}")]
        public ActionResult<GetGolferResponse> GetGolfer(Guid GolferID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var golfer = _golferService.GetGolfer(GolferID);
            var handicap = golfer.Handicap;
            var totalMatches = _statisticService.GetGolferTotalMatches(GolferID);
            var totalFriends = _relationshipService.CountFriends(GolferID);
            string relationship = "";
            if (currentGolfer.Id == GolferID)
            {
                relationship = "IsYou";
            }
            else
            {
                relationship = _relationshipService.GetRelationship(currentGolfer.Id, GolferID);
            }
            GetGolferResponse response = new GetGolferResponse
            {
                Golfer = new MinimizedGolfer
                {
                    Avatar = golfer.Avatar,
                    Cover = golfer.Cover,
                    FullName = golfer.FirstName + " " + golfer.LastName,
                    Handicap = golfer.Handicap,
                    ID = golfer.Id,
                    IDX = golfer.Handicap
                },
                Relationship = relationship,
                TotalFriends = totalFriends,
                TotalMatches = totalMatches
            };
            return Ok(response);
        }

        /// <summary>
        /// Lấy hình ảnh của người dùng
        /// </summary>
        /// <param name="GolferID">ID người dùng</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Photos/{startIndex}")]
        public ActionResult<List<string>> GetGolferPhotos(Guid GolferID, int startIndex)
        {
            List<string> photoURL = _photoService.GetUserPhotos(GolferID, startIndex);
            return Ok(photoURL);
        }
        /// <summary>
        /// Thêm sân hoặc loacation yêu thích
        /// </summary>
        /// <param name="courseOrLocationID">ID sân hoặc ID location</param>
        /// <returns></returns>
        [HttpPut("Favorite/{courseOrLocationID}")]
        public ActionResult<bool> FavoriteCourse(Guid courseOrLocationID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_golferService.FavoriteCourse(currentGolfer.Id, courseOrLocationID).Result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết người dùng
        /// </summary>
        /// <param name="GolferID">ID người dùng</param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Profile")]
        public ActionResult<FullProfileResponse> GetGolferProfile(Guid GolferID)
        {
            var golfer = _golferService.GetGolfer(GolferID);
            var fullprofile = _profileService.GetProfile(golfer);
            return fullprofile;
        }
        /// <summary>
        /// Lấy danh sách nhóm tham gia của người dùng
        /// </summary>
        /// <param name="GolferID">ID người dùng</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Groups/{startIndex}")]
        public ActionResult<List<Group>> GetUserGroup(Guid GolferID, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var groups = _groupService.GetListGroupByGolferId(golfer.Id, startIndex);
            return Ok(groups);
        }

        /// <summary>
        /// tìm kiếm người dùng hệ thống
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Golfer/search/{startIndex}")]
        public ActionResult<List<MinimizedGolferResponse>> SearchGolfer(string searchKey, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _golferService.SearchGolferByName(golfer.Id, searchKey == null ? "" : searchKey.Trim().ToLower(), startIndex);
            return Ok(result.Result);
        }

        /// <summary>
        /// Lấy danh sách gợi ý kết bạn
        /// </summary>
        /// <param name="phoneNumbers">Danh sách số điện thoại truy cập trong danh bạ</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Golfer/SuggestAddFriend/{startIndex}")]
        public ActionResult<List<MinimizedGolferResponse>> SuggestAddFriend( int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _golferService.SuggestAddFriend(golfer.Id, startIndex);
            return Ok(result);
        }

        [HttpGet("Golfer/{golferID}/Friend/search/{startIndex}")]
        public ActionResult<List<MinimizedGolfer>> SearchFriend(Guid golferID, [FromQuery]string searchKey, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _relationshipService.SearchFriend(golferID, searchKey == null ? "" : searchKey.Trim().ToLower(), startIndex);
            return Ok(result);
        }

        /// <summary>
        /// Xếp hạng người dùng
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Ranking/{filter}/{startIndex}")]
        public ActionResult<RankingResponse> RankingGolfer(RankFilter filter, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _golferService.Ranking(filter, startIndex, golfer.Id);
            return Ok(result);
        }


        /// <summary>
        /// APi dữ liệu biểu đồ
        /// </summary>
        /// <param name="GolferID"></param>
        /// <param name="dateRangeFilter"></param>
        /// <param name="amountHoles"></param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Charts/{dateRangeFilter}/{amountHoles}")]
        public ActionResult<ChartsResponse> GetGolferCharts(Guid GolferID, DateRangeFilter dateRangeFilter, int amountHoles, [FromQuery] DateTime startTime, [FromQuery] DateTime finishTime)
        {
            var charts = _chartService.GetGolferCharts(GolferID, dateRangeFilter, amountHoles, startTime, finishTime);
            return Ok(charts);
        }

        /// <summary>
        /// Lấy các mỗi quan hệ (Gửi yêu cầu, nhận yêu cầu, đã xác nhận)(cái này ko chắc chắn)
        /// </summary>
        /// <param name="GolferID">Id người dùg</param>
        /// <param name="relationshipRequestStatus"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Relationship/{relationshipRequestStatus}/{startIndex}")]
        public ActionResult<GetRelatedGolfers> GetRelatedGolfers(Guid GolferID, RelationshipRequestStatus relationshipRequestStatus, int startIndex)
        {
            var golfers = _relationshipService.GetRelationshipGolfers(GolferID, relationshipRequestStatus, startIndex);
            return Ok(new GetRelatedGolfers
            {
                GolferID = GolferID,
                Golfers = golfers
            });
        }

        /// <summary>
        /// API gửi yêu cầu,hủy yêu cầu, xóa ,blook bạn bè
        /// </summary>
        /// <param name="GolferID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{GolferID}/Relationship/{request}")]
        public ActionResult ChangerRelationshipWithGolfer(Guid GolferID, ChangeRelationshipRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            bool status = _relationshipService.ChangeRelationship(currentGolfer.Id, GolferID, request);
            return Ok(status);
        }

        /// <summary>
        /// Lấy dữ liệu màn hình thống  kê thành tích
        /// </summary>
        /// <param name="GolferID">Id người dùng</param>
        /// <param name="dateRangeFilter">Bộ lọc thời gian , số trận muốn thống kê</param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Statistics/{dateRangeFilter}")]
        public ActionResult<Statistics> GetGolferStatistics(Guid GolferID, DateRangeFilter dateRangeFilter, [FromQuery] DateTime startTime, [FromQuery] DateTime finishTime)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var golferStatistics = _statisticService.GetGolferStatistics(GolferID, dateRangeFilter, startTime, finishTime);
            return Ok(golferStatistics);
        }

        //GET: api/Golfers/${GolferID}/Statistic/${dateRangeFilter}/{achievementFilter}/{startIndex}
        /// <summary>
        /// Lấy dữ liệu các trận đấu theo achievementFilter(có par, có brirdie, Eagle,Albatross, Condor)
        /// </summary>
        /// <param name="GolferID">Id người dùng</param>
        /// <param name="dateRangeFilter">Lọc theo thời gian</param>
        /// <param name="achievementFilter">lọc theo Achieverment</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Statistic/{dateRangeFilter}/{achievementFilter}/{startIndex}")]
        public ActionResult<List<MinimizedScorecard>> GetGolferAchievementScorecard(Guid GolferID, DateRangeFilter dateRangeFilter, AchievementFilter achievementFilter, int startIndex, [FromQuery] DateTime startTime, [FromQuery] DateTime finishTime)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<MinimizedScorecard> scorecards = _statisticService.GetGolferAchievementScorecards(GolferID, dateRangeFilter, achievementFilter, startTime, finishTime, startIndex);
            return Ok(scorecards);
        }

        ////GET: api/Golfers/${GolferID}/Scorecards/Pending/{startIndex}
        ////GET: api/Golfers/${GolferID}/Scorecards/Pending/{startIndex}
        ///// <summary>
        ///// Lấy danh sách scorecard chờ đăng(Cacsi này cần xem lại)
        ///// </summary>
        ///// <param name="GolferID">Id người dùng</param>
        ///// <param name="startIndex"></param>
        ///// <returns></returns>
        //[HttpGet("{GolferID}/Scorecards/Pending/{startIndex}")]
        //public ActionResult<List<MinimizedScorecard>> GetGolferPendingScorecards(Guid GolferID, int startIndex)
        //{
        //    var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
        //    List<MinimizedScorecard> scorecards = _scorecardService.GetGolferPeddingScorecards(GolferID, startIndex);
        //    return Ok(scorecards);
        //}
        /// <summary>
        /// Lấy các trận Chờ đăng hoặc tất cả (luyện tập và đã đăng)
        /// </summary>
        /// <param name="GolferID">Định danh ngueofi dùng</param>
        /// <param name="scorecardType">Loại bảng điểm(nhập sau khi chơi và trong khi chơi)</param>
        /// <param name="startIndex"></param>
        /// <returns>Danh sách các bảng điểm thu gọn</returns>
        [HttpGet("{GolferID}/Scorecards/{scorecardType}/{startIndex}")]
        public ActionResult<List<MinimizedScorecard>> GetGolferScorecards(Guid GolferID, FilterScorecardTypeRequest scorecardType, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<MinimizedScorecard> scorecards = _scorecardService.GetGolferScorecards(GolferID, scorecardType, startIndex);
            return Ok(scorecards);
        }

        /// <summary>
        /// Lấy tổng số trận đấu trong các sân của người dùng
        /// </summary>
        /// <param name="GolferID">Định danh người dùng</param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Scorecards/GroupByCourses/{startIndex}")]
        public ActionResult<List<GroupScorecardsByCourse>> GroupGolferScorecardsByCourses(Guid GolferID, int startIndex, FilterScorecardTypeRequest filter)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<GroupScorecardsByCourse> groupScorecardsByCourses = _scorecardService.GroupGolferScorecardsByCourses(GolferID, startIndex, filter);
            return groupScorecardsByCourses;
        }  

        /// <summary>
        /// Tìm kiếm trong các sân đã chơi 
        /// </summary>
        /// <param name="GolferID"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Search/Scorecards/GroupByCourses")]
        public ActionResult<List<GroupScorecardsByCourse>> GroupGolferScorecardsByCourses(Guid GolferID, int startIndex, FilterScorecardTypeRequest filter,[FromQuery]string searchKey)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<GroupScorecardsByCourse> groupScorecardsByCourses = _scorecardService.SearchGolferScorecardsByCourses(GolferID,startIndex, filter, searchKey == null ? "" : searchKey.Trim().ToLower());
            return groupScorecardsByCourses;
        }

        /// <summary>
        /// Lọc các trận đấu(bảng điểm) trong một sân
        /// </summary>
        /// <param name="GolferID">Định danh người dùng</param>
        /// <param name="CourseID">Định danh sân</param>
        /// <param name="scorecardType">Loại bảng điểm</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Scorecards/Courses/{CourseID}/{scorecardType}")]
        public ActionResult<List<MinimizedScorecard>> GetScorecardsByCourse(Guid GolferID, Guid CourseID, FilterScorecardTypeRequest scorecardType, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<MinimizedScorecard> scorecards = _scorecardService.GetScorecardsByCourse(CourseID, scorecardType, GolferID, startIndex);
            return Ok(scorecards);
        }

        //GET: api/Golfers/{GolferID}/Handicap
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GolferID"></param>
        /// <returns></returns>
        [HttpGet("{GolferID}/Handicap")]
        public ActionResult<GetGolferHandicapResponse> GetHandicap(Guid GolferID)
        {
            var golfer = _golferService.GetMinimizedGolfer(GolferID);
            if (golfer == null)
                return NotFound("Golfer is't exit");
            List<MiniScorecard> scorecards = _scorecardService.GetHandicapDetails(GolferID);
            GetGolferHandicapResponse response = new GetGolferHandicapResponse
            {
                Owner = golfer,
                IDXHandicap = golfer.Handicap,
                Scorecards = scorecards
            };
            if (scorecards.Count != 0)
            {
                response.HDCRevisionDate = scorecards[0].Date;
            }
            return Ok(response);
        }

        /// <summary>
        /// danh sách các san đã chơi và handicap của nó
        /// </summary>
        /// <param name="GolferID">định danh người dùng</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{GolferID}/CourseHandicaps/{startIndex}")]
        public ActionResult<GetCourseHandicapResponse> GetCourseHandicaps(Guid GolferID,int startIndex)
        {
            var golfer = _golferService.GetMinimizedGolfer(GolferID);
            if (golfer == null)
                return NotFound("Golfer is't exit");
            GetCourseHandicapResponse response = _handicapService.GetCoursePlayedHandicap(GolferID,startIndex);
            return Ok(response);
        }

        /// <summary>
        /// Tinh HDC của một sân
        /// </summary>
        /// <param name="GolferID">ĐỊnh danh người dùng</param>
        /// <param name="CourseID">Định danh sân</param>
        /// <returns></returns>
        [HttpGet("{GolferID}/CourseHandicap/{CourseID}")]
        public ActionResult<CourseHandicap> GetCourseHandicap(Guid GolferID, Guid CourseID)
        {
            var golfer = _golferService.GetMinimizedGolfer(GolferID);
            if (golfer == null)
                return NotFound("Not found Golfer");
            var course = _courseRepository.Get(CourseID);
            if (course == null)
                return NotFound("Not found course");
            CourseHandicap courseHandicapDetail = new CourseHandicap();
            courseHandicapDetail.ID = course.ID;
            courseHandicapDetail.CourseName = course.Name;
            courseHandicapDetail.Cover = course.Cover;
            courseHandicapDetail.PhoneNumber = course.Location.PhoneNumber;
            courseHandicapDetail.Address = course.Location.Address;
            foreach (var i in course.Tees)
            {
                CourseHandicapDetail courseHandicap = new CourseHandicapDetail();
                courseHandicap.Tee = i;
                courseHandicap.CourseHDC = _handicapService.CalculateCourseHandicap(i, golfer.Handicap);
                courseHandicapDetail.CourseHandicapDetails.Add(courseHandicap);
            }
            return Ok(courseHandicapDetail);
        }

        /// <summary>
        /// Gửi yêu cầu membership
        /// </summary>
        /// <param name="courseID">Định danh sân</param>
        /// <returns></returns>
        [HttpPost("membership/{courseID}")]
        public ActionResult<Boolean> AddCourseMemberShip( Guid courseID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var course = _courseRepository.Get(courseID);
            if (course == null)
                throw new NotFoundException("Course isn't exit!");
            return Ok(_courseMemberShipService.SendMemberShipRequest(currentGolfer.Id));
        }

        /// <summary>
        /// Kiểm tra xem có phải là thành viên sân hay không
        /// </summary>
        /// <param name="courseID"></param>
        /// <returns></returns>
        [HttpPost("ismembership")]
        public ActionResult<Boolean> IsCourseMemberShip(Guid courseID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var course = _courseRepository.Get(courseID);
            if (course == null)
                throw new NotFoundException("Course isn't exit!");
            return Ok(_courseMemberShipService.IsMemberShip(currentGolfer.Id));
        }

        //User setting
        /// <summary>
        /// Lấy thông tin cài đặt
        /// </summary>
        /// <returns></returns>
        [HttpGet("MySetting")]
        public ActionResult<UserSettingRequest> MySetting()
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _golferService.MySetting(currentGolfer.Id);
        }
    }
}
