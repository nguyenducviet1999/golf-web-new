using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Golf.Core.Dtos.Controllers.CourseAdminController.Requests
{
    public class EditCoursePhotosRequest
    {
        public List<IFormFile> Files { get; set; }
    }
}