using Golf.Domain.Shared.Scorecard;
using System.Linq;
namespace Golf.Core.Common.Charts
{
    public class ScoreDistribution
    {
        public int BirdiesAndUpperScores { get; set; } = 0;
        public int Pars { get; set; } = 0;
        public int Bogeys { get; set; } = 0;
        public int DoubleBogeysAndLowerScores { get; set; } = 0;

        public void CalculateScoreDistribution(Achievements achievements)
        {
            this.BirdiesAndUpperScores = achievements.Birdies.Count() + achievements.HoleInOnes.Count() + achievements.Albatrosses.Count() + achievements.Condors.Count() + achievements.Eagles.Count();
            this.Pars = achievements.Pars.Count();
            this.Bogeys = achievements.Bogeys.Count();
            this.DoubleBogeysAndLowerScores = achievements.DoubleBogeys.Count() + achievements.TripleBogeys.Count() + achievements.LowerScores.Count();
        }
    }
}