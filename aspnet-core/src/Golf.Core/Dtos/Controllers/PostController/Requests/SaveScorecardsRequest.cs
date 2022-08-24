using System.Collections.Generic;
using System.Linq;
using System;

using Golf.Core.Dtos.Request;
using Golf.Domain.Shared.Post;
using Golf.Core.Dtos.Controllers.ScorecardController.Requests;

namespace Golf.Core.Dtos.Controllers.PostController.Requests
{
    public class SaveScorecardsRequest : IRequest
    {
        public string Content { get; set; }
        public List<Guid> TagIDs { get; set; } = new List<Guid>();
        public PostPrivacyLevel Privacy { get; set; }
        public List<string> Photos { get; set; } = new List<string>();
        public Guid GroupID { get; set; } = Guid.Empty;
        public List<SaveScorecardRequest> Scorecards { get; set; } = new List<SaveScorecardRequest>();

        public override bool Validate()
        {
            if (this.Scorecards.Count() <= 0)
            {
                return false;
            }
            return true;
        }
    }
}