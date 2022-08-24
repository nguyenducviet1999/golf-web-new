using Golf.Domain.Bookings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class ProductRepository : BaseRepository<Product>
    {
        public ProductRepository(GolfDbContext context) : base(context)
        {

        }
        public IEnumerable<Product> FindDetail(Expression<Func<Product, bool>> predicate)
        {
            return Entities.Include(p=>p.Course).Include(p=>p.Course.Location).Where(predicate);
        }
    }
}
