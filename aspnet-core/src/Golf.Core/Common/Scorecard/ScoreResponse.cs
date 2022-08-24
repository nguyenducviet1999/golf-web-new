using System;
using System.Collections.Generic;

using Golf.Domain.Scorecard;
using Golf.Domain.Shared.Scorecard;

namespace Golf.Core.Common.Scorecard
{
    public class ScoreResponse
    {

        public int Grosses { get; set; }
        public double RealGrosses { get; set; }
        public Achievements Achievements { get; set; }
        public List<Hole> Holes { get; set; } = new List<Hole>();
        public List<double> ParsAverage { get; set; } = new List<double>();
    }
}