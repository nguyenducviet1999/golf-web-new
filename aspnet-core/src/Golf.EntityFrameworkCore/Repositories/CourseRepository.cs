using Golf.Domain.Courses;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class CourseRepository : BaseRepository<Course>
    {
        public CourseRepository(GolfDbContext context) : base(context)
        {

        }
        public IEnumerable<Course> Find(Expression<Func<Course, bool>> predicate)
        {
            return Entities.Include(c=>c.Location).Include(c=>c.Owner).Where(predicate);
        }
        public IEnumerable<Course> FindDetail(Expression<Func<Course, bool>> predicate)
        {
            //var tmp = Context.Locations.ToList();
            //return Entities.Where(predicate);
            return Entities.Include(course => course.Location).Where(predicate);
        }
        public IEnumerable<Course> GetAllDetail()
        {
            //var tmp = Context.Locations.ToList();
            //return Entities.Where(predicate);
            return Entities.Include(course => course.Location);
        }

        public virtual Course Get(Guid id)
        {
            //var tmp = Context.Locations.ToList();
            var result = Entities.Find(id);
            if(result!=null)
            {
                result.Location = Context.Locations.Find(result.LocationID);
            }    
            return result;

        }



    }
}
