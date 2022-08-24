using System.Collections.Generic;
using System;

using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Post;
using Golf.Core.Common.Scorecard;

namespace Golf.Core.Common.Post
{
    public class CommentResponse
    {
        public Guid ID { get; set; }
        public Guid? ParentID { get; set; }
        public string Content { get; set; }
        public MinimizedGolfer Owner { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<MinimizedGolfer> TagGolfers { get; set; } = new List<MinimizedGolfer>();
        public int TotalLikes { get; set; }
        public int TotalReplies { get; set; }
        public bool IsLiked { get; set; }
        public string PhotoID { get; set; }

    }
}