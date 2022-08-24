using System.Collections.Generic;
using System;

using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Post;
using Golf.Core.Common.Scorecard;

namespace Golf.Core.Common.Post
{
    public class CommentDetailResponse : CommentResponse
    {
        public List<CommentResponse> Child { get; set; } = new List<CommentResponse>();
        
    }
}