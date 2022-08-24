using System;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;

using Golf.EntityFrameworkCore.Repositories;
using Golf.Core.Dtos.Controllers.GolfersController.Requests;
using Golf.Core.Common.Scorecard;
using Golf.Domain.Scorecard;
using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Scorecard;
using Golf.Domain.Shared;

namespace Golf.Services
{
    public class StatisticService
    {
        private readonly ScorecardRepository _scorecardRepository;
        private readonly ScorecardService _scorecardService;
        private readonly IMapper _mapper;

        public StatisticService(ScorecardRepository scorecardRepository, ScorecardService scorecardService, IMapper mapper)
        {
            _scorecardRepository = scorecardRepository;
            _scorecardService = scorecardService;
            _mapper = mapper;
        }

        /// <summary>
        /// L?y d? li?u th?ng kê thành tích
        /// </summary>
        /// <param name="GolferID"></param>
        /// <param name="dateRangeFilter"></param>
        /// <param name="startTime"></param>
        /// <param name="finishTime"></param>
        /// <returns></returns>
        public Statistics GetGolferStatistics(Guid GolferID, DateRangeFilter dateRangeFilter, DateTime startTime, DateTime finishTime)
        {
            var result = new Statistics
            {
                GolferID = GolferID,
                TotalMatches = 0,
                Achievements = new Achievements(),
                BestRound = null
            };
            var scorecards = _scorecardRepository.GetScorecardsByDateRangeFilter(GolferID, dateRangeFilter, startTime, finishTime);
            if (scorecards.Count() != 0)
            {
                foreach (Scorecard scorecard in scorecards)
                {
                    result.Achievements.AddAchievements(scorecard.Achievements);
                    result.TotalMatches += 1;
                };
                var highestScore = scorecards.Min(m => m.Grosses);
                Scorecard bestScorecard = scorecards.Find(scorecard => scorecard.Grosses == highestScore);
                result.BestRound = _scorecardService.GetMinimizedScorecard(bestScorecard);
            }
            return result;
        }

        /// <summary>
        /// L?c các tr?n ??u theo Thành tích
        /// </summary>
        /// <param name="GolferID">??nh danh ng??i dùng</param>
        /// <param name="dateRangeFilter">B? l?c th?i gian</param>
        /// <param name="achievementFilter">B? l?c thành tích</param>
        /// <param name="startTime"></param>
        /// <param name="finishTime"></param>
        /// <param name="startIndex">V? trí phân trang</param>
        /// <returns></returns>
        public List<MinimizedScorecard> GetGolferAchievementScorecards(Guid GolferID, DateRangeFilter dateRangeFilter, AchievementFilter achievementFilter, DateTime startTime, DateTime finishTime, int startIndex)
        {
            var scorecards = _scorecardRepository.GetAchievemetScorecardsWithDateRange(GolferID, dateRangeFilter, achievementFilter, startTime, finishTime);
            var tmp = scorecards.Skip(startIndex).Take(Const.PageSize).ToList();
            List<MinimizedScorecard> result = new List<MinimizedScorecard>();
            foreach (var i in tmp)
            {
                result.Add(_mapper.Map<Scorecard, MinimizedScorecard>(i));
            }
            return result;
        }
        /// <summary>
        /// L?y t?ng s? tr?n ??u (?ã ???c xác nh?n)
        /// </summary>
        /// <param name="GolferID"></param>
        /// <returns></returns>
        public int GetGolferTotalMatches(Guid GolferID)
        {
            //var totalFirstMatches = _scorecardRepository.Find(sc => sc.OwnerID == GolferID).OrderBy(sc=>sc.CreatedDate).Take(10).Count();
            var totalConfirmMatches = _scorecardRepository.CountAll(sc => sc.OwnerID == GolferID && sc.IsConfirmed ==true && sc.Type != ScorecardType.Pending);
            return totalConfirmMatches;
        }
    }
}