namespace Golf.Core.Common.Charts
{
    public class ChartsResponse
    {
        public FairwayHit FairwayHit { get; set; } = new FairwayHit();
        public ScoreDistribution ScoreDistribution { get; set; } = new ScoreDistribution();
        public GreenInRegulation GIR { get; set; } = new GreenInRegulation();
        public ParAverages ParAverages { get; set; } = new ParAverages();
        public ScoresCharts ScoresCharts { get; set; } = new ScoresCharts();
    }
}