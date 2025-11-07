using Bookify.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class Repository<T, TKey> : IRepository<T, TKey> where T : class
{
    protected readonly AppDbContext _db;
    internal DbSet<T> dbSet; 

    public Repository(AppDbContext db)
    {
        _db = db;
        dbSet = _db.Set<T>(); 
    }

   public async Task<IEnumerable<T>> GetAllAsync(
       Expression<Func<T, bool>>? filter = null,
       params Expression<Func<T, object>>[] includeProperties)
   {
      IQueryable<T> query = dbSet;

       if (filter != null)
           query = query.Where(filter);

       foreach (var includeProperty in includeProperties)
           query = query.Include(includeProperty);

       return await query.ToListAsync();
   }

   public async Task<T?> GetByIdAsync(TKey id, bool tracked = true)
   {
           if (tracked)
                  return await dbSet.FindAsync(id);
    
          return await dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id));
       }


    public async Task AddAsync(T entity) => await dbSet.AddAsync(entity);


    public Task UpdateAsync(T entity) { dbSet.Update(entity); return Task.CompletedTask; }


    public async Task DeleteAsync(TKey id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        dbSet.Remove(entity);
    }


    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbSet.Where(predicate).ToListAsync();
    }
}