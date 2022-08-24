using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Golf.Core.Dtos.Controllers.CoursesController.Requests
{
    public class EditCourseReviewRequest
    {
        public Guid CourseReviewID { get; set; }
        public double Point { get; set; }
        public string Content { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}