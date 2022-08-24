using Golf.Domain.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
   
    public class EventRepository : BaseRepository<Event>
    {
        public EventRepository(GolfDbContext context) : base(context)
        {

        }
        public IEnumerable<Event> Find(Expression<Func<Event, bool>> predicate)
        {
            return Entities.Include(ev=> ev.Course).Include(ev=>ev.Owner).Include(ev=>ev.Course.Location).Where(predicate);
        }
    }
}
