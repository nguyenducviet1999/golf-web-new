using System.Collections.Generic;
using System;

using Golf.Domain.Courses;

namespace Golf.Core.Dtos.Controllers.CoursesController.Responses
{
    public class GetCourseTeesResponse
    {
        public Guid CourseID { get; set; }
        public List<Tee> Tees { get; set; }
    }
}