using Golf.Domain.Shared.Post;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Post
{
    public class ReferenceObject
    {
        public Guid ID { get; set; }
        public ReferenceObjectType? Type { get; set; }
        public string? WebLink { get; set; }
    }

}
