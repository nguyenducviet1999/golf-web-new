using System;

using Golf.Domain.Courses;
using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Scorecard;

namespace Golf.Core.Common.Scorecard
{
    public class MinimizedScorecard
    {
        public Guid ID { get; set; }
        public MinimizedGolfer Owner { get; set; }
        public MinimizedGolfer? CreatedBy { get; set; }
        public int Grosses { get; set; }
        public int CoursePars { get; set; }
        public string CourseName { get; set; }
        public DateTime Date { get; set; }
        public DateTime? PostDate { get; set; }
        public Achievements Achievements { get; set; }
        //public bool IsVerified { get; set; }
        public double? HDCBefore { get; set; }
        public double? HDCAfter { get; set; }
        //public bool IsPending { get; set; }
        public bool IsConfirmed { get; set; }
        public ScorecardType Type { get; set; }
        public Tee Tee { get; set; }
    }

}