using Golf.Domain.Tournaments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{

    public class TournamentRepository : BaseRepository<Tournament>
    {
        public TournamentRepository(GolfDbContext context) : base(context)
        {

        }
        public IEnumerable<Tournament> Find(Expression<Func<Tournament, bool>> predicate)
        {
            return Entities.Include(t => t.Owner).Where(predicate);
        }
    }
}
