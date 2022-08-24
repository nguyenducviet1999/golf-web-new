using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using Golf.Domain.Shared.Post;

namespace Golf.Core.Dtos.Controllers.PostController.Requests
{
    public class EditPostRequest
    {
        public string Content { get; set; }
        public List<Guid> TagIDs { get; set; } = new List<Guid>();
        public List<string> photos { get; set; } = new List<string>();
        public List<string> ClientCurrentPhotoIDs { get; set; } = new List<string>();
        public PostPrivacyLevel Privacy { get; set; }
    }
}