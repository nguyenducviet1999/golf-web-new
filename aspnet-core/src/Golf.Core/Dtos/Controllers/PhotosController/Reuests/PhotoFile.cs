
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Golf.Core.Dtos.Controllers.CourseAdminController.Requests
{
    public class PhotoFile
    {
        public IFormFile photo { get; set; }
    }
}