using System;
using System.Collections.Generic;
using Golf.Core.Common.Golfer;
using Golf.Core.Common.Scorecard;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Scorecard;

namespace Golf.Core.Dtos.Controllers.GolfersController.Responses
{
    public class GetGolferHandicapResponse
    {
      
         public MinimizedGolfer Owner { get; set; }
        public double IDXHandicap { get; set; } = 0;
        public Nullable<DateTime> HDCRevisionDate { get; set; }
        public List<MiniScorecard> Scorecards { get; set; } = new List<MiniScorecard>();
    }
    public class MiniScorecard 
    {
        public Guid ID { get; set; }
        public int Grosses { get; set; }
        public int CoursePars { get; set; }
        public string CourseName { get; set; }
        public DateTime Date { get; set; }
        public DateTime? PostDate { get; set; }
        public Achievements Achievements { get; set; }
       
        public Tee Tee { get; set; }
    }
}