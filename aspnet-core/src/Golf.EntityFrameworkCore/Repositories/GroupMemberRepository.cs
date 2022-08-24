using Golf.Domain.SocialNetwork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class GroupMemberRepository: BaseRepository<GroupMember>
    {
        public GroupMemberRepository(GolfDbContext context) : base(context)
        {

        }
        public IEnumerable<GroupMember> Find(Expression<Func<GroupMember, bool>> predicate)
        {
            return Entities.Include(gm=>gm.Group).Where(predicate);
        }
        public IEnumerable<GroupMember> FindWithGolfwer(Expression<Func<GroupMember, bool>> predicate)
        {
            return Entities.Include(gm=>gm.Golfer).Where(predicate);
        }
    }
}
