using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Tuanament;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.TournamentController.Response
{
    public class TournamentMemberResponse //: MinimizedGolfer
    {
        public Guid MemberID = new Guid();
        public TournamentMemberStatus MemberStatus { get; set; }
        public MinimizedGolfer Golfer { get; set; }
        
        
        //public  MinimizedGolfer Golfer= new MinimizedGolfer();
    }
}
