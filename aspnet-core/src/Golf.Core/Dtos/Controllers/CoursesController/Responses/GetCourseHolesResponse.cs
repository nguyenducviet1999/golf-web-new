using System.Collections.Generic;
using System;

using Golf.Core.Common.Course;

namespace Golf.Core.Dtos.Controllers.CoursesController.Responses
{
    public class GetCourseHolesResponse
    {
        public Guid CourseID { get; set; }
        public List<CourseHoleResponse> CourseHoles { get; set; }
    }
}