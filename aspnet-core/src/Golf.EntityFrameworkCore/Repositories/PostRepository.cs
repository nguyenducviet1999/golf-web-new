using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System;

using Golf.Domain.Post;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class PostRepository : BaseRepository<Post>
    {
        public PostRepository(GolfDbContext context) : base(context)
        {

        }
        
        public IEnumerable<Post> Find(Expression<Func<Post, bool>> predicate)
        {
            return Entities.Include(p => p.Owner).Include(p => p.Group).Where(p=>p.DeleteDate==null&&p.DeleteBy==null).Where(predicate);
        }
        public IEnumerable<Post> FindAdmin(Expression<Func<Post, bool>> predicate)
        {
            return Entities.Include(p => p.Owner).Include(p => p.Group).Where(predicate);
        }
        public Post GetPost(Guid PostID)
        {
            return Entities
                .Include(post => post.Group)
                .Include(post => post.Owner)
                .Include(post => post.Parent)
                .Where(post => post.ID == PostID)
                .FirstOrDefault();
        }
    }
}