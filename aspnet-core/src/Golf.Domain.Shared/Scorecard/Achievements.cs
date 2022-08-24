using System.Collections.Generic;

namespace Golf.Domain.Shared.Scorecard
{
    public class Achievements
    {
        public List<int> HoleInOnes { get; set; }
        public List<int> Condors { get; set; }
        public List<int> Albatrosses { get; set; }
        public List<int> Eagles { get; set; }
        public List<int> Birdies { get; set; }
        public List<int> Pars { get; set; }
        public List<int> Bogeys { get; set; }
        public List<int> DoubleBogeys { get; set; }
        public List<int> TripleBogeys { get; set; }
        public List<int> LowerScores { get; set; }

        public void AddAchievements(Achievements otherAchievements)
        {
            this.HoleInOnes.AddRange(otherAchievements.HoleInOnes);
            this.Condors.AddRange(otherAchievements.Condors);
            this.Albatrosses.AddRange(otherAchievements.Albatrosses);
            this.Eagles.AddRange(otherAchievements.Eagles);
            this.Birdies.AddRange(otherAchievements.Birdies);
            this.Pars.AddRange(otherAchievements.Pars);
            this.Bogeys.AddRange(otherAchievements.Bogeys);
            this.DoubleBogeys.AddRange(otherAchievements.DoubleBogeys);
            this.TripleBogeys.AddRange(otherAchievements.TripleBogeys);
            this.LowerScores.AddRange(otherAchievements.LowerScores);
        }

        public Achievements()
        {
            this.HoleInOnes = new List<int>();
            this.Condors = new List<int>();
            this.Albatrosses = new List<int>();
            this.Eagles = new List<int>();
            this.Birdies = new List<int>();
            this.Pars = new List<int>();
            this.Bogeys = new List<int>();
            this.DoubleBogeys = new List<int>();
            this.TripleBogeys = new List<int>();
            this.LowerScores = new List<int>();
        }

        public int GetBestHole()
        {
            if (this.HoleInOnes.Count > 0)
            {
                return this.HoleInOnes[this.HoleInOnes.Count - 1];
            }

            if (this.Condors.Count > 0)
            {
                return this.Condors[this.Condors.Count - 1];
            }

            if (this.Albatrosses.Count > 0)
            {
                return this.Albatrosses[this.Albatrosses.Count - 1];
            }

            if (this.Eagles.Count > 0)
            {
                return this.Eagles[this.Eagles.Count - 1];
            }

            if (this.Birdies.Count > 0)
            {
                return this.Birdies[this.Birdies.Count - 1];
            }

            if (this.Pars.Count > 0)
            {
                return this.Pars[this.Pars.Count - 1];
            }

            if (this.Bogeys.Count > 0)
            {
                return this.Bogeys[this.Bogeys.Count - 1];
            }

            if (this.DoubleBogeys.Count > 0)
            {
                return this.DoubleBogeys[this.DoubleBogeys.Count - 1];
            }

            if (this.TripleBogeys.Count > 0)
            {
                return this.TripleBogeys[this.TripleBogeys.Count - 1];
            }

            return this.LowerScores[this.LowerScores.Count - 1];
        }
    }
}