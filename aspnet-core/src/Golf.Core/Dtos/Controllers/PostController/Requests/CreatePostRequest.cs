using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

using Golf.Domain.Shared.Post;
using Golf.Domain.Post;

namespace Golf.Core.Dtos.Controllers.PostController.Requests
{
    public class CreatePostRequest
    {
        public string Content { get; set; }
        public List<Guid> TagIDs { get; set; } = new List<Guid>();
        public List<Guid> ScorecardIDs { get; set; } = new List<Guid>();
        public List<Guid> EventIDs { get; set; } = new List<Guid>();
        public List<Guid> TournamentIDs { get; set; } = new List<Guid>();
        public List<string> Photos { get; set; } = new List<string>();
        public PostPrivacyLevel Privacy { get; set; }
        public Guid GroupID { get; set; } = Guid.Empty;
        public PostAction? PostFeeling { get; set; }
        //public List<PostObject>? PostObjects { get; set; } = new List<PostObject>();
    }
}