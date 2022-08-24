using Golf.Domain.Shared.Scorecard;

namespace Golf.Core.Dtos.Controllers.ScorecardController.Responses
{
    public class ScorecardHoleResponse
    {
        public int Index { get; set; }
        public int AdjustScore { get; set; }
        public int Score { get; set; }
        //public int RealScore { get; set; }
        public int DistanceMeter { get; set; }
        public int DistanceYard { get; set; }
        public int Over { get; set; }
        public int Par { get; set; }
        public Fairway Fairway { get; set; }
        public string ClubOfTee { get; set; }
        public int Putts { get; set; }
        public int SandShots { get; set; }
        public int PenaltyStrokes { get; set; }
        public string Note { get; set; }
        public GIR GIR { get; set; }
    }
}