using Golf.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class CourseReviewRepository : BaseRepository<CourseReview>
    {
       public CourseReviewRepository(GolfDbContext context) : base(context)
        {

        }
        public IEnumerable<CourseReview> Find(Expression<Func<CourseReview, bool>> predicate)
        {
            return Entities.Include(cr=>cr.Owner).Where(predicate);
        }
    }
}
