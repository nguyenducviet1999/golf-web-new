using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

using Golf.EntityFrameworkCore.Repositories;

using Golf.Domain.GolferData;
using Golf.Domain.Scorecard;
using Golf.Domain.Courses;
using Golf.Domain.Shared;
using Golf.Domain.Shared.Course;
using Golf.Domain.Shared.Scorecard;

using Golf.Services.Helpers;

using Golf.Core.Common.Scorecard;
using Golf.Core.Common.Golfer;
using Golf.Core.Exceptions;

using Golf.Core.Dtos.Controllers.GolfersController.Requests;
using Golf.Core.Dtos.Controllers.GolfersController.Responses;
using Golf.Core.Dtos.Controllers.ScorecardController.Requests;
using Golf.Core.Dtos.Controllers.ScorecardController.Responses;
using Golf.Services.Notifications;
using Golf.EntityFrameworkCore;

namespace Golf.Services
{
    public class ScorecardService
    {
        private readonly ScorecardRepository _scorecardRepository;
        private readonly ScorecardVoteRepository _scorecardVoteRepository;
        private readonly CourseRepository _courseRepository;
        private readonly GolferRepository _golferRepository;
        private readonly SystemSettingRepository _systemSettingRepository;
        private readonly HandicapService _handicapService;
        private readonly NotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly DatabaseTransaction _databaseTransaction;

        public ScorecardService(
            ScorecardVoteRepository scorecardVoteRepository,
            SystemSettingRepository systemSettingRepository,
            DatabaseTransaction databaseTransaction,
            NotificationService notificationService,
            ScorecardRepository scorecardRepository,
            CourseRepository courseRepository,
            HandicapService handicapService,
            GolferRepository golferRepository,
            IMapper mapper)
        {
            _scorecardVoteRepository = scorecardVoteRepository;
            _systemSettingRepository = systemSettingRepository;
            _databaseTransaction = databaseTransaction;
            _notificationService = notificationService;
            _scorecardRepository = scorecardRepository;
            _courseRepository = courseRepository;
            _golferRepository = golferRepository;
            _handicapService = handicapService;
            _mapper = mapper;
        }

        private int GetDistance(TeeColor teeColor, CourseHole courseHole)
        {
            switch (teeColor)
            {
                case TeeColor.xFF000000:
                    return courseHole.BlackTeeDistance;
                case TeeColor.xFF2385F8:
                    return courseHole.BlueTeeDistance;
                case TeeColor.xFFFB2B2B:
                    return courseHole.RedTeeDistance;
                case TeeColor.xFFFFFFFF:
                    return courseHole.WhiteTeeDistance;
                default:
                    throw new BadRequestException("Tee color error");
            }
        }

        public List<MiniScorecard> GetHandicapDetails(Guid GolferID)
        {
            var tmp = _scorecardRepository.FindScorecard(sc => sc.OwnerID == GolferID);
            if (tmp.Count() <= 0)
                return new List<MiniScorecard>();
            var t = tmp.OrderByDescending(s => s.Date).Take(20);
            var result = t.OrderBy(s => s.Grosses).Take(_handicapService.GetUseableInvalidScorecard(t.Count()));
            return result.Select(sc => GetMiniScorecard(sc)).ToList();
        }
        //public double CalculateHandicap(Guid GolferID)
        //{
        //    var tmp = _scorecardRepository.FindScorecard(sc => sc.OwnerID == GolferID);
        //    if (tmp.Count() <= 10)
        //        return 0;
        //    var t = tmp.OrderByDescending(s => s.Date).Take(20);

        //    if (t.Count() >= 10 && t.Count() < 15)
        //    {
        //        var result = t.OrderBy(s => s.Grosses).Take(3);
        //        //foreach(var i in result)
        //        //{
        //        //    var handicapDifferential = (i.Grosses - i.Tee.CourseRating) * 113 / i.Tee.SlopeRating;
        //        //}
        //        return Math.Round(result.Average(i => (i.Grosses - i.Tee.CourseRating) * 113 / i.Tee.SlopeRating),1);
        //    }
        //    if (t.Count() >= 15 && t.Count() < 20)
        //    {
        //        var result = t.OrderBy(s => s.Grosses).Take(6);
        //        return Math.Round(result.Average(i => (i.Grosses - i.Tee.CourseRating) * 113 / i.Tee.SlopeRating), 1);

        //    }
        //    else
        //    {
        //        var result = t.OrderBy(s => s.Grosses).Take(10);
        //        return Math.Round(result.Average(i => (i.Grosses - i.Tee.CourseRating) * 113 / i.Tee.SlopeRating), 1);
        //    }
        //    return 0;
        //}
        /// <summary>
        /// Lấy dư liệu thu gọn cuả bảng điểm
        /// </summary>
        /// <param name="scorecard">Dữ liệu bảng điểm</param>
        /// <returns></returns>
        public MinimizedScorecard GetMinimizedScorecard(Scorecard scorecard)
        {
            return new MinimizedScorecard
            {
                ID = scorecard.ID,
                Owner = _mapper.Map<MinimizedGolfer>(scorecard.Owner),
                CreatedBy = _mapper.Map<MinimizedGolfer>(_golferRepository.Get((Guid)scorecard.CreatedBy)),
                Grosses = scorecard.Grosses,
                CourseName = scorecard.Course.Name,
                CoursePars = scorecard.Course.Par,
                Date = scorecard.Date,
                HDCAfter = scorecard.HandicapAfter,
                HDCBefore = scorecard.HandicapBefore,
                //IsVerified = scorecard.IsVerified,
                //IsPending = scorecard.IsPending,
                Tee = scorecard.Tee,
                IsConfirmed = scorecard.IsConfirmed,
                Type = scorecard.Type == ScorecardType.Posted && scorecard.IsConfirmed == false ? ScorecardType.Waiting : scorecard.Type,
                Achievements = scorecard.Achievements,
                PostDate = scorecard.PostDate,
            };
        }

        public MiniScorecard GetMiniScorecard(Scorecard scorecard)
        {
            return new MiniScorecard
            {
                ID = scorecard.ID,
                Grosses = scorecard.Grosses,
                CourseName = scorecard.Course.Name,
                CoursePars = scorecard.Course.Par,
                Date = scorecard.Date,
                PostDate = scorecard.PostDate,
                Achievements = scorecard.Achievements,
            };
        }

        /// <summary>
        /// Lưu bảng điểm
        /// </summary>
        /// <param name="currentGolfer">định danh người dùng hiện thời</param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Scorecard SaveScorecard(Golfer currentGolfer, SaveScorecardRequest request)
        {
            var course = _courseRepository.Get(request.CourseID);
            if (course == null)
                throw new NotFoundException("Not Found Course!");
            var golfer = _golferRepository.Get(request.OwnerID);
            if (golfer == null)
                throw new NotFoundException("Not Found Golfer");
            var tee = course.Tees.ToList().Find(tee => tee.Color == request.TeeColor);
            var adjustScore = 0;
            var handicap = golfer.Handicap;
            var courseHandicap = _handicapService.CalculateCourseHandicap(tee, handicap);
            var score = CalculateScore(courseHandicap, course, request, handicap);
            foreach (var i in score.Holes)
            {
                adjustScore = adjustScore + i.AdjustGrossScore;
            }
            var system36Handicap = _handicapService.CalculateSystem36Handicap(score.Achievements);
            var scorecard = new Scorecard
            {
                ID = Guid.NewGuid(),
                Date = request.Date,
                Owner = golfer,
                Course = course,
                IsConfirmed = false,
                HandicapDifferential = _handicapService.CalculateHandicapDifferential(tee, adjustScore),
                //IsVerified = false,
                //IsPending = currentGolfer.Id == request.OwnerID ? false : true,
                InputType = request.InputType,
                Type = currentGolfer.Id == request.OwnerID ? request.Type : ScorecardType.Pending,
                Tee = tee,
                Achievements = score.Achievements,
                Grosses = score.Grosses,
                RealGrosses = score.RealGrosses,
                Holes = score.Holes,
                HandicapBefore = handicap,
                HandicapAfter = null,
                CourseHandicapBefore = courseHandicap,
                CourseHandicapAfter = null,
                System36Handicap = system36Handicap,
                BestHole = score.Achievements.GetBestHole(),
                AmountHoles = request.AmountHoles,
                ParsAverage = score.ParsAverage
            };
            if (scorecard.Type == ScorecardType.Posted)
            {
                var numberScorecardAutoConfirm = 0;
                var setting = _systemSettingRepository.FirstOrDefault();
                if (setting == null)
                {
                    numberScorecardAutoConfirm = 10;
                }
                else
                {
                    numberScorecardAutoConfirm = setting.Setting.NumberScorecardAutoConfirm;
                }
                if (_scorecardRepository.Find(sc => sc.OwnerID == golfer.Id && sc.Type == ScorecardType.Posted).Count() < numberScorecardAutoConfirm)
                {
                    scorecard.IsConfirmed = true;
                    scorecard.HandicapAfter = _handicapService.CalculateHandicap(golfer, scorecard.HandicapDifferential, scorecard.HandicapBefore);
                    scorecard.Owner.Handicap = (double)scorecard.HandicapAfter;
                    scorecard.CourseHandicapAfter = scorecard.CourseHandicapBefore;
                    if (scorecard.HandicapAfter != scorecard.CourseHandicapBefore)
                    {
                        scorecard.CourseHandicapAfter = _handicapService.CalculateCourseHandicap(tee, (double)scorecard.HandicapAfter);
                    }
                    scorecard.PostDate = DateTime.Now;
                }
            }
            _scorecardRepository.Add(scorecard);
            _golferRepository.Update(scorecard.Owner);
            return scorecard;
        }

        /// <summary>
        /// xác nhận bảng điểm
        /// </summary>
        /// <param name="ScorecardID">định danh bảng điểm</param>
        /// <param name="golfer">Đối tượng confỉm</param>
        public void ConfirmScorecard(Guid ScorecardID, Golfer golfer)
        {
            var scorecard = _scorecardRepository.Get(ScorecardID);
            var scorecardVote = _scorecardVoteRepository.CountAll(sv=>sv.ScorecardID==ScorecardID);
            if (scorecard == null)
            {
                throw new NotFoundException("Not found Scorecard");
            }
            if (scorecard.IsConfirmed == true)
                return;
            scorecard.Owner = _golferRepository.Get(scorecard.OwnerID);
            if (scorecard.Owner != golfer)
            {
                if (scorecardVote == 0)
                {
                    scorecard.HandicapAfter = _handicapService.CalculateHandicap(scorecard.Owner, scorecard.HandicapDifferential, scorecard.HandicapBefore);
                    scorecard.Owner.Handicap = (double)scorecard.HandicapAfter;
                    scorecard.CourseHandicapAfter = scorecard.CourseHandicapBefore;
                    if (scorecard.HandicapAfter != scorecard.CourseHandicapBefore)
                    {
                        scorecard.CourseHandicapAfter = _handicapService.CalculateCourseHandicap(scorecard.Tee, (double)scorecard.HandicapAfter);
                    }
                }
                scorecard.PostDate = DateTime.Now;
                scorecard.IsConfirmed =true;
                _scorecardRepository.UpdateEntity(scorecard);
                _golferRepository.UpdateEntity(scorecard.Owner);
            }
            else
            {
                throw new BadRequestException("Can't confirm your own scorecard");
            }
        }

        /// <summary>
        /// Thông kê bảng điểm
        /// </summary>
        /// <param name="course"></param>
        /// <param name="request"></param>
        /// <param name="handicap"></param>
        /// <returns></returns>
        public ScoreResponse CalculateScore(double courseHandicap, Course course, SaveScorecardRequest request, double handicap)
        {
            var tee = course.Tees.ToList().Find(tee => tee.Color == request.TeeColor);
            var score = new ScoreResponse
            {
                Grosses = 0,
                RealGrosses = 0,
                Achievements = new Achievements(),
                Holes = new List<Hole>(),
                ParsAverage = new List<double>()
            };
            List<List<int>> pars = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>()
            };
            AchievementsIndex achievementsIndex = new AchievementsIndex();
            List<CourseHole> courseHoles = course.CourseHoles.ToList();
            for (var holeIndex = 0; holeIndex < request.Holes.Count(); holeIndex++)
            {
                var courseHole = courseHoles[holeIndex];
                var grosses = request.Holes[holeIndex].Grosses;
                var par = courseHole.Par;
                var MaxGrossScore = _handicapService.GetMaxGrossScore(courseHandicap, par);
                score.Achievements.AddAchievements(_handicapService.CheckAchievements(grosses, par, holeIndex + 1));
                //score.RealGrosses += grosses;
                score.Grosses += grosses;
                if (par == 3)
                {
                    pars[0].Add(grosses);
                }
                else if (par == 4)
                {
                    pars[1].Add(grosses);
                }
                else if (par == 5)
                {
                    pars[2].Add(grosses);
                }
                var putts = request.Holes[holeIndex].Putts;
                var hole = new Hole()
                {
                    Index = holeIndex + 1,
                    //RealGrosses = grosses,
                    Over = grosses - par,
                    Grosses = grosses,
                    AdjustGrossScore = grosses > MaxGrossScore ? MaxGrossScore : grosses,
                    Note = request.Holes[holeIndex].Note,
                    StrokeIndex = courseHole.StrokeIndex,
                    ClubOfTee = request.Holes[holeIndex].ClubOfTee,
                };
                if (request.Holes[holeIndex].Fairway != null)
                    hole.Fairway = (Fairway)request.Holes[holeIndex].Fairway;
                if (request.Holes[holeIndex].PenaltyStrokes != null)
                    hole.PenaltyStrokes = (int)request.Holes[holeIndex].PenaltyStrokes;
                if (request.Holes[holeIndex].SandShots != null)
                    hole.SandShots = (int)request.Holes[holeIndex].SandShots;
                if (putts != null)
                {
                    hole.GIR = CheckGIR(grosses, par, (int)putts);
                    hole.Putts = (int)putts;
                }
                score.Holes.Add(hole);
            }
            score.RealGrosses = (double)score.Grosses - handicap;//CourseHandicap 
            score.ParsAverage.Add(pars[0].Average());
            score.ParsAverage.Add(pars[1].Average());
            score.ParsAverage.Add(pars[2].Average());
            return score;
        }

        private GIR CheckGIR(int grosses, int par, int putts)
        {
            if (putts == 0 || grosses == 0)
            {
                return GIR.None;
            }
            if (grosses == 1)
            {
                return GIR.Hit;
            }
            if (grosses <= par)
            {
                if (par == 3 && grosses - putts == 1)
                {
                    return GIR.Hit;
                }
                else if (par == 4 && grosses - putts <= 2)
                {
                    return GIR.Hit;
                }
                else if (par == 5 && grosses - putts <= 3)
                {
                    return GIR.Hit;
                }
                else if (par == 6 && grosses - putts <= 4)
                {
                    return GIR.Hit;
                }
            }
            return GIR.Miss;
        }

        public List<MinimizedScorecard> GetGolferPeddingScorecards(Guid GolferID, int startIndex)
        {
            return _scorecardRepository.FindScorecard(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Pending)
                .OrderByDescending(scorecard => scorecard.Date)
                .Skip(startIndex)
                .Take(Const.PageSize)
                .Select(scorecard => GetMinimizedScorecard(scorecard))
                .ToList();
        }
        /// <summary>
        /// Bảng điểm của người dùng
        /// </summary>
        /// <param name="GolferID">Định danh người dùng</param>
        /// <param name="request">Giá trị lọc</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns></returns>
        public List<MinimizedScorecard> GetGolferScorecards(Guid GolferID, FilterScorecardTypeRequest request, int startIndex)
        {
            switch (request)
            {
                case FilterScorecardTypeRequest.All:
                    {
                        var reusult = _scorecardRepository.FindScorecard(scorecard => scorecard.OwnerID == GolferID && scorecard.Type != ScorecardType.Pending);
                        if (reusult.Count() > 0)
                            return reusult
                                 .OrderByDescending(scorecard => scorecard.Date)
                                 .Skip(startIndex)
                                 .Take(Const.PageSize)
                                 .Select(scorecard => GetMinimizedScorecard(scorecard))
                                 .ToList();
                        else return new List<MinimizedScorecard>();
                    }

                case FilterScorecardTypeRequest.Pending:
                    {
                        var reusult = _scorecardRepository.FindScorecard(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Pending);
                        if (reusult.Count() > 0)
                            return reusult
                            .OrderByDescending(scorecard => scorecard.Date)
                            .Skip(startIndex)
                            .Take(Const.PageSize)
                            .Select(scorecard => GetMinimizedScorecard(scorecard))
                            .ToList();
                        else return new List<MinimizedScorecard>();
                    }
                case FilterScorecardTypeRequest.Posted:
                    {

                        var reusult = _scorecardRepository.FindScorecard(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Posted && scorecard.IsConfirmed ==true);
                        if (reusult.Count() > 0)
                            return reusult
                            .OrderByDescending(scorecard => scorecard.Date)
                            .Skip(startIndex)
                            .Take(Const.PageSize)
                            .Select(scorecard => GetMinimizedScorecard(scorecard))
                            .ToList();
                        else return new List<MinimizedScorecard>();
                    }
                case FilterScorecardTypeRequest.Practice:
                    {

                        var reusult = _scorecardRepository.FindScorecard(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Practice);
                        if (reusult.Count() > 0)
                            return reusult
                            .OrderByDescending(scorecard => scorecard.Date)
                            .Skip(startIndex)
                            .Take(Const.PageSize)
                            .Select(scorecard => GetMinimizedScorecard(scorecard))
                            .ToList();
                        else return new List<MinimizedScorecard>();
                    }
                default:
                    throw new BadRequestException("Undefined scorecard type");
            }
        }
        /// <summary>
        /// Lấy các bảng điểm của người dùng trên một sân nào đó
        /// </summary>
        /// <param name="GolferID"></param>
        /// <param name="startIndex"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<GroupScorecardsByCourse> GroupGolferScorecardsByCourses(Guid GolferID, int startIndex, FilterScorecardTypeRequest filter)
        {
            switch (filter)
            {
                case FilterScorecardTypeRequest.All:
                    {
                        return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type != ScorecardType.Pending)
                   .GroupBy(scorecard => scorecard.Course)
                   .Select(gb =>
                       new GroupScorecardsByCourse
                       {
                           CourseID = gb.Key.ID,
                           CourseName = gb.Key.Name,
                           TotalScorecards = gb.Count()
                       })
                   .Skip(startIndex)
                   .Take(Const.PageSize)
                   .ToList();
                    }
                case FilterScorecardTypeRequest.Practice:
                    {
                        return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Practice)
                   .GroupBy(scorecard => scorecard.Course)
                   .Select(gb =>
                       new GroupScorecardsByCourse
                       {
                           CourseID = gb.Key.ID,
                           CourseName = gb.Key.Name,
                           TotalScorecards = gb.Count()
                       })
                   .Skip(startIndex)
                   .Take(Const.PageSize)
                   .ToList();
                    }
                case FilterScorecardTypeRequest.Posted:
                    {
                        return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Posted)
                   .GroupBy(scorecard => scorecard.Course)
                   .Select(gb =>
                       new GroupScorecardsByCourse
                       {
                           CourseID = gb.Key.ID,
                           CourseName = gb.Key.Name,
                           TotalScorecards = gb.Count()
                       })
                   .Skip(startIndex)
                   .Take(Const.PageSize)
                   .ToList();
                    }
                case FilterScorecardTypeRequest.Pending:
                    {
                        return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Pending)
                   .GroupBy(scorecard => scorecard.Course)
                   .Select(gb =>
                       new GroupScorecardsByCourse
                       {
                           CourseID = gb.Key.ID,
                           CourseName = gb.Key.Name,
                           TotalScorecards = gb.Count()
                       })
                   .Skip(startIndex)
                   .Take(Const.PageSize)
                   .ToList();
                    }
                default: return new List<GroupScorecardsByCourse>();
            }

        }

        /// <summary>
        /// Tìm kiếm trong các sân đã chơi
        /// </summary>
        /// <param name="GolferID">định danh người dùng</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <param name="filter">Giá trị bộ lọc</param>
        /// <param name="searchKey">Từ khóa tìm kiếm</param>
        /// <returns></returns>
        public List<GroupScorecardsByCourse> SearchGolferScorecardsByCourses(Guid GolferID, int startIndex, FilterScorecardTypeRequest filter, string searchKey)
        {
            switch (filter)
            {
                case FilterScorecardTypeRequest.All:
                    {
                        return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type != ScorecardType.Pending && scorecard.Course.Name.ToLower().Trim().Contains(searchKey.Trim().ToLower()))
                   .GroupBy(scorecard => scorecard.Course)
                   .Select(gb =>
                       new GroupScorecardsByCourse
                       {
                           CourseID = gb.Key.ID,
                           CourseName = gb.Key.Name,
                           TotalScorecards = gb.Count()
                       })
                   .Skip(startIndex)
                   .Take(Const.PageSize)
                   .ToList();
                    }
                case FilterScorecardTypeRequest.Posted:
                    {
                        return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Posted && scorecard.Course.Name.ToLower().Contains(searchKey.Trim().ToLower()))
                   .GroupBy(scorecard => scorecard.Course)
                   .Select(gb =>
                       new GroupScorecardsByCourse
                       {
                           CourseID = gb.Key.ID,
                           CourseName = gb.Key.Name,
                           TotalScorecards = gb.Count()
                       })
                   .Skip(startIndex)
                   .Take(Const.PageSize)
                   .ToList();
                    }
                case FilterScorecardTypeRequest.Practice:
                    {
                        return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Practice && scorecard.Course.Name.ToLower().Contains(searchKey.Trim().ToLower()))
                   .GroupBy(scorecard => scorecard.Course)
                   .Select(gb =>
                       new GroupScorecardsByCourse
                       {
                           CourseID = gb.Key.ID,
                           CourseName = gb.Key.Name,
                           TotalScorecards = gb.Count()
                       })
                   .Skip(startIndex)
                   .Take(Const.PageSize)
                   .ToList();
                    }
                case FilterScorecardTypeRequest.Pending:
                    {
                        return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Pending && scorecard.Course.Name.ToLower().Contains(searchKey.Trim().ToLower()))
                   .GroupBy(scorecard => scorecard.Course)
                   .Select(gb =>
                       new GroupScorecardsByCourse
                       {
                           CourseID = gb.Key.ID,
                           CourseName = gb.Key.Name,
                           TotalScorecards = gb.Count()
                       })
                   .Skip(startIndex)
                   .Take(Const.PageSize)
                   .ToList();
                    }
                default: return new List<GroupScorecardsByCourse>();
            }

        }

        /// <summary>
        /// Lọc các trận đấu theo sân 
        /// </summary>
        /// <param name="CourseID">định danh sân</param>
        /// <param name="request">Bộ lọc</param>
        /// <param name="GolferID">định danh người dùng</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns></returns>
        public List<MinimizedScorecard> GetScorecardsByCourse(Guid CourseID, FilterScorecardTypeRequest request, Guid GolferID, int startIndex)
        {
            var course = _courseRepository.Get(CourseID);
            if (course != null)
            {
                switch (request)
                {
                    case FilterScorecardTypeRequest.All:
                        {
                            return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Course == course && scorecard.Type != ScorecardType.Pending)
                                 .OrderByDescending(scorecard => scorecard.Date)
                                 .Skip(startIndex)
                                 .Take(Const.PageSize)
                                 .Select(scorecard => GetMinimizedScorecard(scorecard))
                                 .ToList();
                        }
                    case FilterScorecardTypeRequest.Posted:
                        {
                            return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Posted && scorecard.Course == course)
                                .OrderByDescending(scorecard => scorecard.Date)
                                .Skip(startIndex)
                                .Take(Const.PageSize)
                                .Select(scorecard => GetMinimizedScorecard(scorecard))
                                .ToList();
                        }
                    case FilterScorecardTypeRequest.Pending:
                        {
                            return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Pending && scorecard.Course == course)
                                .OrderByDescending(scorecard => scorecard.Date)
                                .Skip(startIndex)
                                .Take(Const.PageSize)
                                .Select(scorecard => GetMinimizedScorecard(scorecard))
                                .ToList();
                        }
                    case FilterScorecardTypeRequest.Practice:
                        {
                            return _scorecardRepository.Find(scorecard => scorecard.OwnerID == GolferID && scorecard.Type == ScorecardType.Practice && scorecard.Course == course)
                                .OrderByDescending(scorecard => scorecard.Date)
                                .Skip(startIndex)
                                .Take(Const.PageSize)
                                .Select(scorecard => GetMinimizedScorecard(scorecard))
                                .ToList();
                        }
                    default:
                        throw new BadRequestException("Undefined scorecard type");
                }
            }
            throw new Exception("Can't find course");
        }

        public List<MinimizedScorecard> GetMinimizedScorecardsByIDs(List<Guid> scorecardIDs)
        {
            return _scorecardRepository
                .Find(scorecard => scorecardIDs.Contains(scorecard.ID))
                .Select(scorecard => GetMinimizedScorecard(scorecard))
                .ToList();
        }

        /// <summary>
        /// Xóa trận đâng chờ
        /// </summary>
        /// <param name="scorecardID"></param>
        /// <returns></returns>
        public bool DeletePendingScoreCard(Guid scorecardID)
        {
            var scorecard = _scorecardRepository.Get(scorecardID);
            if (scorecard.Type != ScorecardType.Pending&& scorecard.Type != ScorecardType.Practice)
            {
                throw new ForbiddenException("Cann't delete scorecard!");
            }
            _scorecardRepository.RemoveEntity(scorecard);
            return true;
        }

        /// <summary>
        /// Lấy dữ liệu chi tiết bảng điểm
        /// </summary>
        /// <param name="ScorecardID">Định danh bảng điểm</param>
        /// <returns></returns>
        public GetScorecardDetailResponse GetScorecardDetail(Guid ScorecardID)
        {
            Scorecard scorecard = _scorecardRepository.FindScorecard(sc => sc.ID == ScorecardID).FirstOrDefault();
            if (scorecard != null)
            {
                ScorecardDetailStatisticsResponse scorecardStatisticsResponse = new ScorecardDetailStatisticsResponse();
                List<ScorecardHoleResponse> holeResponses = new List<ScorecardHoleResponse>();
                List<int> putts = new List<int>();
                foreach (Hole hole in scorecard.Holes)
                {
                    CourseHole courseHole = scorecard.Course.CourseHoles.Find(courseHole => courseHole.Index == hole.Index);
                    int distanceYard = 0;
                    switch (scorecard.Tee.Color)
                    {
                        case TeeColor.xFF000000:
                            {
                                distanceYard = courseHole.BlackTeeDistance;
                                break;
                            }
                        case TeeColor.xFFFB2B2B:
                            {
                                distanceYard = courseHole.RedTeeDistance;
                                break;
                            }
                        case TeeColor.xFF2385F8:
                            {
                                distanceYard = courseHole.BlueTeeDistance;
                                break;
                            }
                        case TeeColor.xFFFFFFFF:
                            {
                                distanceYard = courseHole.WhiteTeeDistance;
                                break;
                            }
                    }

                    if (scorecard.InputType == ScorecardInputType.AfterPlay)
                    {
                        if (hole.Fairway == Fairway.Hit)
                        {
                            scorecardStatisticsResponse.FairwayHit.Hits += 1;
                        }
                        else if (hole.Fairway != Fairway.None)
                        {
                            scorecardStatisticsResponse.FairwayHit.Misses += 1;
                        }

                        if (hole.GIR == GIR.Hit)
                        {
                            scorecardStatisticsResponse.GIR.Hits += 1;
                        }
                        else if (hole.GIR == GIR.Miss)
                        {
                            scorecardStatisticsResponse.GIR.Misses += 1;
                        }
                    }
                    int distanceMeter = DistanceConvert.ConvertYardToMeter(distanceYard);
                    putts.Add(hole.Putts);
                    holeResponses.Add(new ScorecardHoleResponse
                    {
                        Index = hole.Index,
                        ClubOfTee = hole.ClubOfTee,
                        Note = hole.Note,
                        Fairway = hole.Fairway,
                        Putts = hole.Putts,
                        SandShots = hole.SandShots,
                        PenaltyStrokes = hole.PenaltyStrokes,
                        Score = hole.Grosses,
                        // RealScore = hole.RealGrosses,
                        Par = courseHole.Par,
                        AdjustScore = hole.AdjustGrossScore,
                        Over = hole.Over,
                        DistanceMeter = distanceMeter,
                        DistanceYard = distanceYard
                    });
                }
                scorecardStatisticsResponse.ScoreDistribution.CalculateScoreDistribution(scorecard.Achievements);
                scorecardStatisticsResponse.ParAverages.Par3 = scorecard.ParsAverage[0];
                scorecardStatisticsResponse.ParAverages.Par4 = scorecard.ParsAverage[1];
                scorecardStatisticsResponse.ParAverages.Par5 = scorecard.ParsAverage[2];
                GetScorecardDetailResponse scorecardDetail = new GetScorecardDetailResponse
                {
                    Scorecard = new ScorecardDetailResponse
                    {
                        ID = scorecard.ID,
                        CourseName = scorecard.Course.Name,
                        CourseID = scorecard.CourseID,
                        CourseCover = scorecard.Course.Cover,
                        CoursePars = scorecard.Course.Par,
                        InputType = scorecard.InputType,
                        Date = scorecard.Date,
                        Grosses = scorecard.Grosses,
                        RealGrosses = scorecard.RealGrosses,
                        HDCAfter = scorecard.HandicapAfter,
                        HDCBefore = scorecard.HandicapBefore,
                        CourseHandicapAfter = scorecard.CourseHandicapAfter,
                        CourseHandicapBefore = scorecard.CourseHandicapBefore,
                        Holes = holeResponses,
                        //IsPending = scorecard.IsPending,
                        //IsVerified = scorecard.IsVerified,
                        Tee = scorecard.Tee,
                        IsConfirmed = scorecard.IsConfirmed,
                        Type = scorecard.Type,
                        Achievements = scorecard.Achievements,
                        BestHole = scorecard.BestHole,
                        Net = 0,
                        System36Handicap = scorecard.System36Handicap,
                        AmountHoles = scorecard.AmountHoles,
                    },
                    Statistics = scorecardStatisticsResponse,
                    Owner = _mapper.Map<MinimizedGolfer>(scorecard.Owner)
                };
                return scorecardDetail;
            }
            throw new Exception("Can't find scorecard");
        }
    }
}