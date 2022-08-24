using Microsoft.AspNetCore.Http;

namespace Golf.Core.Dtos.Controllers.ProfileController.Requests
{
    public class SetAvatarRequest
    {
        public IFormFile File { get; set; }
    }
}
