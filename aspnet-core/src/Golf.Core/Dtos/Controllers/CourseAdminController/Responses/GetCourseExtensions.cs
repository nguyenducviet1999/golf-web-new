using Golf.Domain.Courses;
using System.Collections.Generic;

namespace Golf.Core.Dtos.Controllers.CourseAdminController.Responses
{
    public class GetExtensions
    {
        public List<CourseExtension> CourseExtensions { get; set; }
    }
}