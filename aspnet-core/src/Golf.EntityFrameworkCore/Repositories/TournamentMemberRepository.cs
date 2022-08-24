using Golf.Domain.Tournaments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{

    public class TournamentMemberRepository : BaseRepository<TournamentMember>
    {
        public TournamentMemberRepository(GolfDbContext context) : base(context)
        {

        }
        public IEnumerable<TournamentMember> FindIncludeGolfer(Expression<Func<TournamentMember, bool>> predicate)
        {
            return Entities.Include(t => t.Golfer).Where(predicate);
        }
        public IEnumerable<TournamentMember> FindIncludeTournament(Expression<Func<TournamentMember, bool>> predicate)
        {
            return Entities.Include(t=>t.Tuanament).Where(predicate);
        } 
        public IEnumerable<TournamentMember> FindDetail(Expression<Func<TournamentMember, bool>> predicate)
        {
            return Entities.Include(t=>t.Tuanament).Include(t => t.Golfer).Where(predicate);
        }
    }
}
