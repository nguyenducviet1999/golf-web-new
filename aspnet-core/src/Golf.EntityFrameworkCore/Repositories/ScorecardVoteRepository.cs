using Golf.Domain.Scorecard;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{

    public class ScorecardVoteRepository : BaseRepository<ScorecardVote>
    {
        public ScorecardVoteRepository(GolfDbContext context) : base(context)
        {

        }

        public IEnumerable<ScorecardVote> Find(Expression<Func<ScorecardVote, bool>> predicate)
        {
            return Entities.Include(p => p.Golfer).Where(predicate);
        }
    }
}
