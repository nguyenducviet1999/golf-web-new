using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

using Golf.Domain.Base;
using Golf.Domain.Shared.Post;

namespace Golf.Domain.Post
{
    public class PostVote : IEntityBase
    {
        public Guid PostID { get; set; }
        public Guid? CommentID { get; set; }
        public VoteType Type { get; set; }
    }
}
