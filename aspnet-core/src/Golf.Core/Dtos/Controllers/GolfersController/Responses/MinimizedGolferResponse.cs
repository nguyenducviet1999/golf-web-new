using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Relationship;
using Golf.Domain.SocialNetwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.GolfersController.Responses
{
    public class MinimizedGolferResponse
    {
        public MinimizedGolfer Golfer { get; set; }
        public string Relationship { get; set; }
        public int NumberOfMutualFriends { get; set; }
    }
}
