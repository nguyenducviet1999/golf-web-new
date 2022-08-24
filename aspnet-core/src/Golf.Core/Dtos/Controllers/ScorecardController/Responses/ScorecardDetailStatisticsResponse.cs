using Golf.Core.Common.Charts;

namespace Golf.Core.Dtos.Controllers.ScorecardController.Responses
{
    public class ScorecardDetailStatisticsResponse
    {
        public ScoreDistribution ScoreDistribution { get; set; }
        public ParAverages ParAverages { get; set; }
        public GreenInRegulation GIR { get; set; }
        public FairwayHit FairwayHit { get; set; }

        public ScorecardDetailStatisticsResponse()
        {
            this.FairwayHit = new FairwayHit();
            this.ScoreDistribution = new ScoreDistribution();
            this.ParAverages = new ParAverages();
            this.GIR = new GreenInRegulation();
        }
    }
}