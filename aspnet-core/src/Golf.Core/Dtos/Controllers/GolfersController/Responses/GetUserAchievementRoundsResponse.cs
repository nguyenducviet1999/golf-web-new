using System.Collections.Generic;

using Golf.Core.Common.Scorecard;

namespace Golf.Core.Dtos.Controllers.GolfersController.Responses
{
    public class GetGolferAchievementScorecardsResponse
    {
        public List<MinimizedScorecard> scorecards { get; set; }
    }
}