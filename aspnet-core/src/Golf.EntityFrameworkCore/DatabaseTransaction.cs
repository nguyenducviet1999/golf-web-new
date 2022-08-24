
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Golf.EntityFrameworkCore
{
    public class DatabaseTransaction
    {
        protected readonly GolfDbContext Context;
        private IDbContextTransaction transaction;

        public DatabaseTransaction(GolfDbContext context)
        {
            Context = context;
        }

        public async Task<bool> Commit()
        {
            await this.transaction.CommitAsync();
            await Context.SaveChangesAsync();
            return true;
        }

        public void BeginTransaction()
        {
            this.transaction = Context.Database.BeginTransaction();
        }

        public async void Rollback()
        {
            await this.transaction.RollbackAsync();
        }
    }
}