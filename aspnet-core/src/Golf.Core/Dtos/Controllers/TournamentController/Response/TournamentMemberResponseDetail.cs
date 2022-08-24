using Golf.Core.Common.Scorecard;
using Golf.Core.Dtos.Controllers.ScorecardController.Responses;
using Golf.Domain.Shared.Golfer.Information;
using Golf.Domain.Shared.Tuanament;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.TournamentController.Response
{
    public class TournamentMemberResponseDetail
    {
        public Guid ID;
        public string Name;
        public string Address;
        public string Image;
        public string PhoneNumber;
        public string Email;
        public int Age;
        public Gender Gender;
        public double Handicap;
        public double AvgScore;
        public int TotalMatch;
        public MinimizedScorecard BestMatch;
       // public TournamentMemberStatus BestMatch;


    }
}
