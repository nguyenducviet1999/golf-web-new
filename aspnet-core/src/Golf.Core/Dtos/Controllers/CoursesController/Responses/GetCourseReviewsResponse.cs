using System.Collections.Generic;
using System;

using Golf.Core.Common.Course;

namespace Golf.Core.Dtos.Controllers.CoursesController.Responses
{
    public class GetCourseReviewsResponse
    {
        public Guid CourseID { get; set; }
        public List<CourseReviewResponse> CourseReviews { get; set; }
    }
}