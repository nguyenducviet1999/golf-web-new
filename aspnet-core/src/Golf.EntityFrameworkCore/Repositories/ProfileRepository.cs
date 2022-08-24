
using Golf.Domain.GolferData;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class ProfileRepository : BaseRepository<Profile>
    {
        public ProfileRepository(GolfDbContext context) : base(context)
        {

        }
    }
}
