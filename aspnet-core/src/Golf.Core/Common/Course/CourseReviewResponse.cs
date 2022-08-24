using Golf.Core.Common.Golfer;
using System.Collections.Generic;

namespace Golf.Core.Common.Course
{
    public class CourseReviewResponse
    {
        public string Content { get; set; }
        public double Point { get; set; }
        public List<string> PhotoNames { get; set; }
        public MinimizedGolfer Golfer { get; set; }
    }
    public class CourseReviewResponses
    {
        public bool AllowReview { get; set; } = false;
        public List<CourseReviewResponse> ListCourseReviewResponse { get; set; } = new List<CourseReviewResponse>();
    }
}