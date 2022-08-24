using Golf.Core.Common.Golfer;
using Golf.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.GolfersController.Responses
{
    public class GetCourseHandicapResponse
    {
        public MinimizedGolfer Golfer { get; set; }
        public double StartHandicap { get; set; }
        public List<CourseHandicap> CourseHandicaps { get; set; } = new List<CourseHandicap>();
}
    public class CourseHandicap
    {
        public Guid ID { get; set; }
        public string Cover { get; set; }
        public string CourseName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<CourseHandicapDetail> CourseHandicapDetails { get; set; } = new List<CourseHandicapDetail>();

    }
    public class CourseHandicapDetail
    {
        public double CourseHDC { get; set; }
        public Tee Tee { get; set; }
    }
}
