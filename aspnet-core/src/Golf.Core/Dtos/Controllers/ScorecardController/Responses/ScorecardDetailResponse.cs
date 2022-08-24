using System;
using System.Collections.Generic;

using Golf.Domain.Courses;
using Golf.Domain.Shared.Scorecard;

namespace Golf.Core.Dtos.Controllers.ScorecardController.Responses
{
    public class ScorecardDetailResponse
    {
        public Guid ID { get; set; }
        public DateTime Date { get; set; }
        public int Grosses { get; set; }
        public double RealGrosses { get; set; }
        public int CoursePars { get; set; }
        public string CourseCover { get; set; }
        public Guid CourseID { get; set; }
        public string CourseName { get; set; }
        public ScorecardInputType InputType { get; set; }
        public Achievements Achievements { get; set; }
        //public bool IsVerified { get; set; }
        public double HDCBefore { get; set; }
        public double? HDCAfter { get; set; }
       // public bool IsPending { get; set; }
        public bool IsConfirmed { get; set; }
        public int BestHole { get; set; }
        public ScorecardType Type { get; set; }
        public int Net { get; set; }
        public Tee Tee { get; set; }
        public double? CourseHandicapAfter { get; set; }
        public double CourseHandicapBefore { get; set; }
        public double System36Handicap { get; set; }
        public List<ScorecardHoleResponse> Holes { get; set; }
        public int AmountHoles { get; set; }
    }
}