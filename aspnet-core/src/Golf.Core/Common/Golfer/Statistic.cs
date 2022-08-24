using System;

using Golf.Core.Common.Scorecard;
using Golf.Domain.Shared.Scorecard;


namespace Golf.Core.Common.Golfer
{
    public class Statistics
    {
        public Guid GolferID { get; set; }
        public int TotalMatches { get; set; }
        public Achievements Achievements { get; set; }
#nullable enable
        public MinimizedScorecard? BestRound { get; set; }
    }
}