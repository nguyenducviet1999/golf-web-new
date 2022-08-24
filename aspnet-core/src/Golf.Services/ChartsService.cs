using System;
using System.Linq;
using System.Collections.Generic;

using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Scorecard;

using Golf.Core.Common.Charts;
using Golf.Core.Dtos.Controllers.GolfersController.Requests;

namespace Golf.Services
{
    public class ChartsService
    {
        private readonly ScorecardRepository _scorecardRepository;

        public ChartsService(ScorecardRepository scorecardRepository)
        {
            _scorecardRepository = scorecardRepository;
        }
        /// <summary>
        /// L?y d? li?u bi?u ??
        /// </summary>
        /// <param name="GolferID"></param>
        /// <param name="dateRangeFilter"></param>
        /// <param name="amountHoles"></param>
        /// <param name="startTime"></param>
        /// <param name="finishTime"></param>
        /// <returns></returns>
        public ChartsResponse GetGolferCharts(Guid GolferID, DateRangeFilter dateRangeFilter, int amountHoles,DateTime startTime,DateTime finishTime)
        {
            ChartsResponse charts = new ChartsResponse();
            List<Scorecard> scorecards = _scorecardRepository.GetScorecardsByDateRangeFilter(GolferID, dateRangeFilter,startTime,finishTime).Where(scorecard => scorecard.AmountHoles == amountHoles).ToList();
            List<int> scores = scorecards.Select(scorecard => scorecard.Grosses).ToList();
            List<double> par3Averages = new List<double>();
            List<double> par4Averages = new List<double>();
            List<double> par5Averages = new List<double>();
            charts.ScoresCharts.Scores = scores;
            if(scores.Count()>0)
            {
                charts.ScoresCharts.Average = scores.Average();
            }
            else
            {
                charts.ScoresCharts.Average = 0;
            }
            scorecards.ForEach(scorecard =>
            {
                charts.ScoreDistribution.CalculateScoreDistribution(scorecard.Achievements);
                par3Averages.Add(scorecard.ParsAverage[0]);
                par4Averages.Add(scorecard.ParsAverage[1]);
                par5Averages.Add(scorecard.ParsAverage[2]);
            });
            charts.ParAverages.Par3 = par3Averages.Count()!=0? par3Averages.Average():0;
            charts.ParAverages.Par4 = par4Averages.Count() != 0 ? par4Averages.Average() : 0;
            charts.ParAverages.Par5 = par5Averages.Count() != 0 ? par5Averages.Average() : 0;
            return charts;
        }
    }
}