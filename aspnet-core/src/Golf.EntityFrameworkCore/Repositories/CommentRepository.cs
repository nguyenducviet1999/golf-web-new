using Golf.Domain.Post;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class CommentRepository : BaseRepository<Comment>
    {
        public CommentRepository(GolfDbContext context) : base(context)
        {

        }
        public IEnumerable<Comment> Find(Expression<Func<Comment, bool>> predicate)
        {
            return Entities.Include(comment=>comment.Golfer).Where(predicate);
        }
    }
}
