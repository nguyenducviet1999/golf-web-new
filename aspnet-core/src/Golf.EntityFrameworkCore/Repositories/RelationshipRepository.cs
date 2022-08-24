using Golf.Domain.SocialNetwork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class RelationshipRepository: BaseRepository<Relationship>
    {
       public RelationshipRepository(GolfDbContext context) : base(context)
        {

        }
        public IEnumerable<Relationship> FindWithGolfer(Expression<Func<Relationship, bool>> predicate)
        {
            return Entities.Include(r=>r.Friend).Include(r=>r.Golfer).Where(predicate);
        }
    }
}
