using System;
using System.Collections.Generic;

using Golf.Core.Common.Golfer;

namespace Golf.Core.Dtos.Controllers.GolfersController.Responses
{
    public class GetRelatedGolfers
    {
        public Guid GolferID { get; set; }
        public List<MinimizedGolfer> Golfers { get; set; }

    }
}