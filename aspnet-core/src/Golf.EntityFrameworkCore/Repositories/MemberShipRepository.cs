using Golf.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class MemberShipRepository : BaseRepository<MemberShip>
    {
        public MemberShipRepository(GolfDbContext context) : base(context)
        {

        }
    }
   
}
