using Golf.Domain.Post;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class PostVoteRepository : BaseRepository<PostVote>
    {
        public PostVoteRepository(GolfDbContext context) : base(context)
        {

        }
    }
}
