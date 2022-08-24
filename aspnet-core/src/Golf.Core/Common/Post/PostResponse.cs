using System.Collections.Generic;
using System;

using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Post;
using Golf.Core.Common.Scorecard;
using Golf.Domain.Post;
using Golf.Core.Dtos.Controllers.EventController.Response;
using Golf.Core.Dtos.Controllers.TournamentController.Response;
using Golf.Domain.Shared.Scorecard;

namespace Golf.Core.Common.Post
{
    public class PostResponse
    {
        public Guid ID { get; set; }
        public MinimizedGolfer Owner { get; set; }
        public List<MinimizedGolfer> TagGolfers { get; set; } = new List<MinimizedGolfer>();
        public string Content { get; set; }
        public List<string> Photos { get; set; } = new List<string>();
        public PostPrivacyLevel Privacy { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public int TotalConfirms { get; set; }
        public int TotalShares { get; set; }
        public int TotalIncorrects { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<MinimizedScorecard> Scorecards { get; set; } = new List<MinimizedScorecard>();
        public MinimizedGolfer ConfirmGolfer { get; set; } 
        public List<EventResponse> Events { get; set; } = new List<EventResponse>();
        public List<TournamentResponse> Tournaments { get; set; } = new List<TournamentResponse>();
        public bool IsLiked { get; set; }
        public ScorecardVoteType? ConfirmType { get; set; }
#nullable enable
        public PostResponse? Parent { get; set; }
        public Guid? ParentID { get; set; }
        public Guid? GroupID { get; set; }
        public string? GroupName { get; set; }
        public PostAction? PostAction { get; set; }
    }
}