namespace Golf.Core.Common.Post
{
    public class VotesOfPostResponse
    {
        public bool IsLiked { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public int TotalConfirms { get; set; }
        public int TotalShares { get; set; }
        public int TotalIncorrects { get; set; }
    }
}