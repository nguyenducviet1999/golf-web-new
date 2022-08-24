using Golf.Domain.Shared.Scorecard;
namespace Golf.Domain.Scorecard
{
    public class Hole
    {
        public int Index { get; set; }
        public int Over { get; set; }
        public int AdjustGrossScore { get; set; }
        public int StrokeIndex { get; set; }
        //public double RealGrosses { get; set; }
        public int Grosses { get; set; }
        public GIR GIR { get; set; }
        public Fairway Fairway { get; set; }
        public string ClubOfTee { get; set; }
        public int Putts { get; set; }
        public int SandShots { get; set; }
        public int PenaltyStrokes { get; set; }
        public string Note { get; set; }
    }
}