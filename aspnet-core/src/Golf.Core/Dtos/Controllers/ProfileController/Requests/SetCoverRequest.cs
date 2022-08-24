using Microsoft.AspNetCore.Http;

namespace Golf.Core.Dtos.Controllers.ProfileController.Requests
{
    public class SetCoverRequest
    {
        public IFormFile File { get; set; }
    }
}
