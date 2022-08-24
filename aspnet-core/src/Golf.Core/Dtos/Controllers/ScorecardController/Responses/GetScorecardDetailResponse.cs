using Golf.Core.Common.Golfer;

namespace Golf.Core.Dtos.Controllers.ScorecardController.Responses
{
    public class GetScorecardDetailResponse
    {
        public MinimizedGolfer Owner { get; set; }
        public ScorecardDetailResponse Scorecard { get; set; }
        public ScorecardDetailStatisticsResponse Statistics { get; set; }
    }
}