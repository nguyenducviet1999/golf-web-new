using Golf.Domain.Post;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Post
{
    public class PostRespone
    {
        public Guid ID { get; set; }
        public GolferRespone Creator { get; set; } = new GolferRespone();
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Content { get; set; }
        public string ListPhotoIDs { get; set; }
        public List<GolferRespone> ListTagGolfers { get; set; } = new List<GolferRespone>();
        public PostRespone Parent { get; set; }
        public Guid? GroupID { get; set; }
        public int Type { get; set; }
        public List<ReferenceObject> ReferenceObject { get; set; } = new List<ReferenceObject>();
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public int TotalConfirms { get; set; }
        public int TotalIncorrect { get; set; }

    }
    public class GolferRespone
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Avata { get; set; }
    }
}
