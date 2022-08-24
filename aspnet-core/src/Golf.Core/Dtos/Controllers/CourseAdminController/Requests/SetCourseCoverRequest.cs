using Microsoft.AspNetCore.Http;

namespace Golf.Core.Dtos.Controllers.CourseAdminController.Requests
{
    public class SetCourseCoverRequest
    {
        public IFormFile CourseCover { get; set; }//một file ảnh
    }
}