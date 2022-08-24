using Golf.Domain.Bookings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace Golf.EntityFrameworkCore.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>
    {
       public TransactionRepository(GolfDbContext context) : base(context)
        {

        }
        public virtual Transaction Get(Guid id)
        {
            return this.FindWithProduct(ts=>ts.ID==id).First();
        }
        public IEnumerable<Transaction> FindWithProduct(Expression<Func<Transaction, bool>> predicate)
        {
            return Entities.Include(t => t.Product).Include(t=>t.Product.Course).Include(t => t.Product.Course.Location).Where(predicate);
        }
    }
}
