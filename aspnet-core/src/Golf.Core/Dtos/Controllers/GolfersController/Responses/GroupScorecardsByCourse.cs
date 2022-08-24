using System;

namespace Golf.Core.Dtos.Controllers.GolfersController.Responses
{
    public class GroupScorecardsByCourse
    {
        public Guid CourseID { get; set; }
        public string CourseName { get; set; }
        public int TotalScorecards { get; set; }
    }
}