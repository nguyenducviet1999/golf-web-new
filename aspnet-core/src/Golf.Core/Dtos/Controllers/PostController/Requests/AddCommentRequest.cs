using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Golf.Core.Dtos.Controllers.PostController.Requests
{
    public class AddCommentRequest
    {
        public string Content { get; set; }
        public List<Guid> TagIDs { get; set; } = new List<Guid>();
        public IFormFile File { get; set; }
    }
}