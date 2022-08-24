using Golf.Core.Common.Course;
using Golf.Domain.Common;
using Golf.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.CoursesController.Responses
{
    public class CourseDetailResponse
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
        public string Address { get; set; }
        public GPSAddress GPSAddress { get; set; }
        public double ReviewPoint { get; set; }
        public List<CourseReviewResponse> CourseReviewResponse { get; set; } = new List<CourseReviewResponse>();
        public List<CourseExtension> CourseExtensions { get; set; }
        public bool IsFavorite { get; set; }
        public List<CourseInformation> MoreInformation { get; set; }
        public string Description { get; set; }

    }
}
