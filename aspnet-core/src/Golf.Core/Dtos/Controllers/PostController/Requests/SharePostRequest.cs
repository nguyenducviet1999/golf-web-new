using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

using Golf.Domain.Shared.Post;

namespace Golf.Core.Dtos.Controllers.PostController.Requests
{
    public class SharePostRequest
    {
        public string Content { get; set; }
        public List<Guid> TagIDs { get; set; } = new List<Guid>();
        public PostPrivacyLevel Privacy { get; set; }
        public Guid GroupID { get; set; } = Guid.Empty;
        //public PostAction PostFeeling { get; set; } 
    }
}