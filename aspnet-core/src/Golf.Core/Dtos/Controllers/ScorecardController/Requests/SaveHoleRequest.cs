using Golf.Domain.Shared.Scorecard;

namespace Golf.Core.Dtos.Controllers.ScorecardController.Requests
{
    public class SaveHoleRequest
    {
        public int Index { get; set; }
        public int Grosses { get; set; }
        public Fairway? Fairway { get; set; }
        public string? ClubOfTee { get; set; }
        public int? Putts { get; set; }
        public int? SandShots { get; set; }
        public int? PenaltyStrokes { get; set; }
        public string? Note { get; set; }
    }
}