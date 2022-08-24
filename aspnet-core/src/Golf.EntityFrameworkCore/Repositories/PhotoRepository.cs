using Golf.Domain.Resources;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class PhotoRepository : BaseRepository<Photo>
    {
        public PhotoRepository(GolfDbContext context) : base(context)
        {

        }
    }
}