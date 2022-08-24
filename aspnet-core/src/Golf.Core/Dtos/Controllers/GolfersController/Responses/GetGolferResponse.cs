using System;

using Golf.Core.Common.Golfer;

namespace Golf.Core.Dtos.Controllers.GolfersController.Responses
{
    public class GetGolferResponse
    {
        public MinimizedGolfer Golfer { get; set; }
        public int TotalFriends { get; set; } = 0;
        public int TotalMatches { get; set; } = 0;
        public string Relationship { get; set; } = "";
    }
}