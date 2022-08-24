using Golf.Domain.Courses;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class CourseExtensionRepository : BaseRepository<CourseExtension>
    {
        public CourseExtensionRepository(GolfDbContext context) : base(context)
        {

        }
    }
}
