using Golf.Domain.SocialNetwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class GroupRepository : BaseRepository<Group>
    {
        public GroupRepository(GolfDbContext context) : base(context)
        {

        }
    }
}
