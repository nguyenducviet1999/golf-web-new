using Golf.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class LocationRepository : BaseRepository<Location>
    {
        public LocationRepository(GolfDbContext context) : base(context)
        {

        }
       
    }
}
