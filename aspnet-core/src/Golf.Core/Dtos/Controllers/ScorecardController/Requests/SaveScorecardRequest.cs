using System;
using System.Collections.Generic;

using Golf.Domain.Shared.Scorecard;
using Golf.Domain.Shared.Course;

namespace Golf.Core.Dtos.Controllers.ScorecardController.Requests
{
    public class SaveScorecardRequest
    {
        public Guid OwnerID { get; set; }
        public Guid CourseID { get; set; }
        public ScorecardInputType InputType { get; set; }
        public ScorecardType Type { get; set; } = ScorecardType.Practice;
        public TeeColor TeeColor { get; set; }
        public DateTime Date { get; set; }
       // public DateTime? PostDate { get; set; }
        public List<SaveHoleRequest> Holes { get; set; } = new List<SaveHoleRequest>();
        public int StartHole { get; set; }
        public int AmountHoles { get; set; }
    }
}