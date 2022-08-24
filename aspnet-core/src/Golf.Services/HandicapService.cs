using System;
using System.Linq;
using System.Collections.Generic;

using Golf.EntityFrameworkCore.Repositories;
using Golf.Core.Common.Scorecard;
using Golf.Domain.Shared.Scorecard;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;
using Golf.Core.Dtos.Controllers.GolfersController.Responses;
using Golf.Domain.Shared;
using AutoMapper;
using Golf.Core.Common.Golfer;

namespace Golf.Services
{
    public class HandicapService
    {
        private readonly ScorecardRepository _scorecardRepository;
        private readonly CourseRepository _courseRepository;
        private readonly GolferRepository _golferRepository;
        private readonly IMapper _mapper;
        public HandicapService(IMapper mapper, ScorecardRepository scorecardRepository, CourseRepository courseRepository, GolferRepository golferRepository)
        {
            _scorecardRepository = scorecardRepository;
            _courseRepository = courseRepository;
            _golferRepository = golferRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Tính ?i?m ch?p c?a sân
        /// </summary>
        /// <param name="tee">Lo?i hình ch?i</param>
        /// <param name="Handicap">?i?m ch?p cá nhân</param>
        /// <returns></returns>
        public double CalculateCourseHandicap(Tee tee, double Handicap)
        {
            return Math.Round(Handicap * tee.SlopeRating / 113, 0);
        }
        /// <summary>
        /// Tính ?i?m ch?p chênh l?ch c?a scorecard
        /// </summary>
        /// <param name="tee">Lo?i hình th?c ch?i</param>
        /// <param name="adjustGrossScore">t?ng ?i?m ?i?u ch?nh</param>
        /// <returns>?i?m ch?p chênh l?ch</returns>
        public double CalculateHandicapDifferential(Tee tee, int adjustGrossScore)
        {
            return Math.Round((adjustGrossScore - tee.CourseRating) * 133 / tee.SlopeRating, 1);
        }
        /// <summary>
        /// Tính ?i?m ch?p c?a ng??i ch?i 
        /// </summary>
        /// <param name="Owner">?ói t??ng ng??i ch?i</param>
        /// <param name="currenthandicapDifferentials">?i?m ch?p chênh l?ch</param>
        /// <param name="Handicap">?i?m ch?p c?a ng??i ch?i</param>
        /// <returns>?i?m ch?p m?i</returns>
        public double CalculateHandicap(Golfer Owner, double currenthandicapDifferentials, double Handicap)
        {
            var invalidScorecards = _scorecardRepository.Find(sc => sc.Owner.Id == Owner.Id && sc.IsConfirmed ==true).OrderByDescending(sc => sc.Date);
            var countScorecardHandicap = GetUseableInvalidScorecard(invalidScorecards.Count());//s? b?ng di?m t?t g?n nh?t dùng ?? tính HDC
            var useableInvalidScorecards = invalidScorecards.OrderBy(sc => sc.HandicapDifferential).Take(countScorecardHandicap).ToList();
            var highestHandicapDifferential = 0.0;
            if (useableInvalidScorecards.Count() == 0)
            {
                var nHandicap = currenthandicapDifferentials * 0.96;
                return Convert.ToDouble(nHandicap.ToString().Substring(0, nHandicap.ToString().IndexOf(".") + 2));
            }
            else
            {
                highestHandicapDifferential = useableInvalidScorecards[useableInvalidScorecards.Count() - 1].HandicapDifferential;
            }

            if (currenthandicapDifferentials > highestHandicapDifferential)
            {
                return Handicap;
            }
            else
            {
                var handicapDifferentials = useableInvalidScorecards.Sum(sc => sc.HandicapDifferential) - highestHandicapDifferential + currenthandicapDifferentials;
                var newHandicap = handicapDifferentials / useableInvalidScorecards.Count() * 0.96;
                var result = Convert.ToDouble(newHandicap.ToString().Substring(0, newHandicap.ToString().IndexOf(".") + 2)); ;
                switch (invalidScorecards.Count())
                {
                    case 3:
                        {
                            return result - 2;
                        }
                    case 4:
                        {
                            return result - 1;
                        }
                    case 6:
                        {
                            return result - 1;
                        }
                    default:
                        {
                            return result;
                        }
                }

            }
        }
        /// <summary>
        /// Tính ?i?m System36Handicap
        /// </summary>
        /// <param name="achievements">D? li?u thông kê b?ng ?i?m</param>
        /// <returns></returns>
        public int CalculateSystem36Handicap(Achievements achievements)
        {
            int point = (achievements.Eagles.Count() + achievements.Birdies.Count() + achievements.Pars.Count()) * 2 + achievements.Bogeys.Count();
            return 36 - point;
        }
        /// <summary>
        /// Tính ?i?m t?i ?a cho môi h?
        /// </summary>
        /// <param name="CourseHandicap">?i?m ch?p c?a sân</param>
        /// <param name="par">?i?m p? c?a h?</param>
        /// <returns></returns>
        public int GetMaxGrossScore(double CourseHandicap, int par)
        {
            if (CourseHandicap < 10)
            {
                return par + 2;
            }
            else if (CourseHandicap >= 10 && CourseHandicap < 20)
            {
                return 7;
            }
            else if (CourseHandicap >= 20 && CourseHandicap < 30)
            {
                return 8;
            }
            else if (CourseHandicap >= 30 && CourseHandicap < 40)
            {
                return 9;
            }
            return 10;
        }
        /// <summary>
        /// Tính s? l??ng b?ng ?i?m có thành tích t?t nh?t ?? tính Handicap
        /// </summary>
        /// <param name="totalInvalidScorecards">T?ng s? b?ng ?i?m th?a mãn ?i?u ki?n tính ?i?m ch?p</param>
        /// <returns></returns>
        public int GetUseableInvalidScorecard(int totalInvalidScorecards)
        {
            if (totalInvalidScorecards == 0)
            {
                return 0;
            }
            else if (totalInvalidScorecards <= 6)
            {
                return 1;
            }
            else if (totalInvalidScorecards <= 8)
            {
                return 2;
            }
            else if (totalInvalidScorecards <= 11)
            {
                return 3;
            }
            else if (totalInvalidScorecards <= 14)
            {
                return 4;
            }
            else if (totalInvalidScorecards <= 16)
            {
                return 5;
            }
            else if (totalInvalidScorecards <= 18)
            {
                return 6;
            }

            else if (totalInvalidScorecards == 19)
            {
                return 7;
            }
            return 8;
        }
        /// <summary>
        /// Th?ng kê ?i?m cho m?i h?
        /// </summary>
        /// <param name="gross">?i?m cho h?</param>
        /// <param name="par">?i?m par c?a h?</param>
        /// <param name="holeIndex">S? th? t? h?</param>
        /// <returns></returns>
        public Achievements CheckAchievements(int gross, int par, int holeIndex)
        {
            Achievements achievements = new Achievements();
            if (par == 0 || gross == 0)
            {
                return achievements;
            }
            if (gross == 1)
            {
                achievements.HoleInOnes.Add(holeIndex);
            }
            else if (par == gross + 4)
            {
                achievements.Condors.Add(holeIndex);
            }
            else if (par == gross + 3)
            {
                achievements.Albatrosses.Add(holeIndex);
            }
            else if (par == gross + 2)
            {
                achievements.Eagles.Add(holeIndex);
            }
            else if (par == gross + 1)
            {
                achievements.Birdies.Add(holeIndex);
            }
            else if (par == gross)
            {
                achievements.Pars.Add(holeIndex);
            }
            else if (par == gross - 1)
            {
                achievements.Bogeys.Add(holeIndex);
            }
            else if (par == gross - 2)
            {
                achievements.DoubleBogeys.Add(holeIndex);
            }
            else if (par == gross - 3)
            {
                achievements.TripleBogeys.Add(holeIndex);
            }
            else
            {
                achievements.LowerScores.Add(holeIndex);
            }
            return achievements;
        }

        /// <summary>
        /// L?y d? li?u trang ?i?m ch?p sân
        /// </summary>
        /// <param name="currentID">??nh danh ng??i dùng</param>
        /// <param name="startIndex">V? trí phân trang các sân ?ã ch?i</param>
        /// <returns></returns>
        public GetCourseHandicapResponse GetCoursePlayedHandicap(Guid currentID, int startIndex)
        {
            var scorecards = _scorecardRepository.Find(sc => sc.OwnerID == currentID).GroupBy(sc => sc.CourseID, sc => sc.Date);
            GetCourseHandicapResponse getCourseHandicapRespone = new GetCourseHandicapResponse();
            var golfer = _golferRepository.Get(currentID);
            getCourseHandicapRespone.Golfer = _mapper.Map<MinimizedGolfer>(golfer);
            getCourseHandicapRespone.StartHandicap = golfer.StartHandicap;
            if (scorecards.Count() == 0)
            {
                return getCourseHandicapRespone;

            }
            var tmp = scorecards.Select(sc => new { CourseID = sc.Key, Time = sc.ToList().Max() });
            var courseIDs = tmp.OrderByDescending(sc => sc.Time).Select(sc => sc.CourseID);
            var courses = _courseRepository.Find(c => courseIDs.Contains(c.ID)).Skip(startIndex).Take(Const.PageSize).ToList();
            if (courses.Count() > 0)
            {

                foreach (Course course in courses)
                {
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
                        courseHandicap.CourseHDC = this.CalculateCourseHandicap(i, getCourseHandicapRespone.Golfer.Handicap);
                        courseHandicapDetail.CourseHandicapDetails.Add(courseHandicap);
                    }
                    getCourseHandicapRespone.CourseHandicaps.Add(courseHandicapDetail);
                }
            }
            return getCourseHandicapRespone;
        }
    }
}