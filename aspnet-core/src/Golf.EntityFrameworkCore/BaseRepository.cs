using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Golf.Domain.Base;

namespace Golf.EntityFrameworkCore
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public readonly GolfDbContext Context;
        public readonly DbSet<TEntity> Entities;

        public BaseRepository(GolfDbContext context)
        {
            Context = context;
            Entities = Context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = Entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity Get(Guid id)
        {
            return Entities.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Entities.ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Where(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Where(predicate).SingleOrDefault();
        }

        public TEntity FirstOrDefault()
        {
            return Entities.SingleOrDefault();
        }

        public void Add(TEntity entity)
        {
            Entities.Add(entity);
            Context.SaveChanges();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Entities.AddRange(entities);
            Context.SaveChanges();
        }

        public void Remove(TEntity entity)
        {
            Entities.Remove(entity);
            Context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Entities.RemoveRange(entities);
        }

        public virtual void RemoveEntity(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                Entities.Attach(entityToDelete);
            }
            Entities.Remove(entityToDelete);
            Context.SaveChanges();
        }

     public virtual void UpdateEntity(TEntity entityToUpdate)
        {
            Entities.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
            Context.SaveChanges();
        }
        public virtual void Update(TEntity entityToUpdate)
        {
            Entities.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
            Context.SaveChanges();
        }
        public int CountAll(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Where(predicate).Count();
        }

        public void SafeAdd(TEntity entity)
        {
            Entities.Add(entity);
        }
        public void SaveChanges()
        {
            Context.SaveChanges();
        }
        public void SafeUpdate(TEntity entityToUpdate)
        {
            Entities.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
            
        }

        public void SafeRemove(TEntity entityToDelete)
        {
            if (entityToDelete is ISafeEntityBase)
            {
                Context.Entry(entityToDelete).State = EntityState.Deleted;
            }
            else
            {
                Entities.Remove(entityToDelete);
            }
        }

        public void SafeRemoveRange(IEnumerable<TEntity> entities)
        {
            Entities.RemoveRange(entities);
        }

    }
}
