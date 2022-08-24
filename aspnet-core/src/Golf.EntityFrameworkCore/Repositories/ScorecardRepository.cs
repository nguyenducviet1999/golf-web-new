using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System;

using Golf.Domain.Scorecard;
using Golf.Domain.Common;
using Golf.Core.Exceptions;
using Golf.Core.Dtos.Controllers.GolfersController.Requests;
using Golf.Domain.Shared.Scorecard;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class ScorecardRepository : BaseRepository<Scorecard>
    {
       
        public ScorecardRepository(GolfDbContext context) : base(context)
        {
            var courses = this.Context.Courses;
            if (courses.Count() > 0)
            {
                courses.ToList();
            }

        }

        public IEnumerable<Scorecard> FindScorecard(Expression<Func<Scorecard, bool>> predicate)
        {
            return Entities.Include(scorecard => scorecard.Course).Include(scorecard => scorecard.Owner).Where(predicate);
        }

        public List<Scorecard> GetScorecardsByDateRangeFilter(Guid GolferID, DateRangeFilter dateRangeFilter, DateTime startTime, DateTime finishTime)
        {
            switch (dateRangeFilter)
            {
                case DateRangeFilter.All:
                    {
                        return Find(sc =>
                            sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                            && sc.IsConfirmed==true)
                                .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                .ToList();
                    }
                case DateRangeFilter.FiveRounds:
                    {
                        return Find(sc =>
                            sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                            && sc.IsConfirmed == true)
                                .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                .Take(5)
                                .ToList();
                    }
                case DateRangeFilter.TenRounds:
                    {
                        return Find(sc =>
                            sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                            && sc.IsConfirmed == true)
                                 .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                 .Take(10)
                                 .ToList();
                    }
                case DateRangeFilter.TwentyRounds:
                    {
                        return Find(sc =>
                            sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                            && sc.IsConfirmed == true)
                                .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                .Take(20)
                                .ToList();
                    }
                case DateRangeFilter.ThisWeek:
                    {
                        var todayWeekDay = DateTime.Now.DayOfWeek;
                        var difFromMonday = 0;
                        if (todayWeekDay == DayOfWeek.Sunday)
                        {
                            difFromMonday = 6;
                        }
                        else
                        {
                            difFromMonday = todayWeekDay - DayOfWeek.Monday;
                        }

                        var firstDayOfWeek = DateTime.Now.AddDays(-difFromMonday);

                        return Find(sc =>
                             sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                             && sc.IsConfirmed == true
                             && sc.Date >= firstDayOfWeek)
                                 .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                 .ToList();
                    }
                case DateRangeFilter.ThisMonth:
                    {
                        var thisYear = DateTime.Now.Year;
                        var thisMonth = DateTime.Now.Month;
                        return Find(sc =>
                            sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                            && sc.IsConfirmed == true
                            && sc.Date.Month == thisMonth
                            && sc.Date.Year == thisYear)
                                .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                .ToList();
                    }
                case DateRangeFilter.ThisQuarter:
                    {
                        var thisYear = DateTime.Now.Year;
                        var thisMonth = DateTime.Now.Month;
                        var quarter = new Quarter(thisMonth);
                        return Find(sc =>
                            sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                            && sc.IsConfirmed == true
                            && (sc.Date.Month >= quarter.StartMonth
                            && sc.Date.Month <= quarter.EndMonth)
                            && sc.Date.Year == thisYear)
                                .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                .ToList();
                    }
                case DateRangeFilter.HalfYear:
                    {
                        var thisMonth = DateTime.Now.Month;
                        var thisYear = DateTime.Now.Year;
                        return Find(sc =>
                            sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                            && sc.IsConfirmed == true
                            && sc.Date.Year == thisYear
                            && (thisMonth < 7 ? sc.Date.Month < 7 : sc.Date.Month > 7))
                                .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                .ToList();
                    }
                case DateRangeFilter.ThisYear:
                    {
                        var thisYear = DateTime.Now.Year;
                        return Find(sc =>
                            sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                            && sc.IsConfirmed == true
                            && sc.Date.Year == thisYear)
                                .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                .ToList();
                    }
                case DateRangeFilter.Custom:
                    {
                        if(DateTime.Compare(startTime,finishTime)>0)
                            throw new BadRequestException("Start time before finish time");
                        var thisYear = DateTime.Now.Year;
                        return Find(sc =>
                            sc.OwnerID == GolferID && sc.Type == ScorecardType.Posted
                            && sc.IsConfirmed == true
                            && DateTime.Compare(sc.Date, startTime) > 0 && DateTime.Compare(sc.Date, finishTime) < 0)
                                .OrderByDescending(sc => Convert.ToDateTime(sc.Date))
                                .ToList();
                    }
                default:
                    throw new BadRequestException("Unknown Filter");
            }
        }

        public List<Scorecard> GetAchievemetScorecardsWithDateRange(Guid GolferID, DateRangeFilter dateRangeFilter, AchievementFilter achievementFilter, DateTime startTime, DateTime finishTime)
        {
            var scorecards = GetScorecardsByDateRangeFilter(GolferID, dateRangeFilter, startTime, finishTime);
            switch (achievementFilter)
            {
                case AchievementFilter.All:
                    {
                        return scorecards;
                    }
                case AchievementFilter.HoleInOne:
                    {
                        return scorecards.FindAll(scorecard => scorecard.Achievements.HoleInOnes.Count() != 0);
                    }
                case AchievementFilter.Condor:
                    {
                        return scorecards.FindAll(scorecard => scorecard.Achievements.Condors.Count() != 0);
                    }
                case AchievementFilter.Albatross:
                    {
                        return scorecards.FindAll(scorecard => scorecard.Achievements.Albatrosses.Count() != 0);
                    }
                case AchievementFilter.Eagle:
                    {
                        return scorecards.FindAll(scorecard => scorecard.Achievements.Eagles.Count() != 0);
                    }
                case AchievementFilter.Birdie:
                    {
                        return scorecards.FindAll(scorecard => scorecard.Achievements.Birdies.Count() != 0);
                    }
                case AchievementFilter.Par:
                    {
                        return scorecards.FindAll(scorecard => scorecard.Achievements.Pars.Count() != 0);
                    }
                default:
                    throw new BadRequestException("Unknown Achievement Filter");
            }
        }
    }
}
