using Data.Persistence;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Application.Interfaces.Repositories;

namespace Data.Implementation.Repositories;

public class GenericRepository : IGenericRepository
{
    private readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<TEntity>> GetAsync<TEntity>(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "") where TEntity : class
    {
        IQueryable<TEntity> query = _dbContext.Set<TEntity>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }
        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync<TEntity>(object id) where TEntity : class
    {
        return await _dbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity?> GetFirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
    {
        return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(filter);
    }

    public async Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class
    {
        if (filter == null) return false;
        return await (_dbContext.Set<TEntity>().AnyAsync(filter));
    }
    
    public async Task<int> InsertAsync<TEntity>(TEntity entity) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        await _dbContext.Set<TEntity>().AddAsync(entity);
        
        await _dbContext.SaveChangesAsync();
        
        var ret = 0;
        
        var key = typeof(TEntity).GetProperties().FirstOrDefault(p => 
            p.CustomAttributes.Any(attr => 
                attr.AttributeType == typeof(KeyAttribute)));

        if (key != null)
        {
            ret = (int)(key.GetValue(entity, null) ?? 0);
        }
        
        return ret;
    }

    public async Task AddMultipleEntityAsync<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entityList);
        
        await _dbContext.Set<TEntity>().AddRangeAsync(entityList);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync<TEntity>(TEntity entityToUpdate) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entityToUpdate);

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateMultipleEntityAsync<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entityList);

        _dbContext.Entry(entityList).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync<TEntity>(object id) where TEntity : class
    {
        var result = await _dbContext.Set<TEntity>().FindAsync(id);

        if (result != null)
        {
            _dbContext.Set<TEntity>().Remove(result);
        }
    }

    public async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
    {
        var entitiesToDelete = await _dbContext.Set<TEntity>().Where(filter).ToListAsync();

        _dbContext.Set<TEntity>().RemoveRange(entitiesToDelete);

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync<TEntity>(TEntity entityToDelete) where TEntity : class
    {
        _dbContext.Set<TEntity>().Remove(entityToDelete);
        
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<bool> RemoveMultipleEntityAsync<TEntity>(IEnumerable<TEntity> removeEntityList) where TEntity : class
    {
        _dbContext.Set<TEntity>().RemoveRange(removeEntityList);

        var affectedRows = await _dbContext.SaveChangesAsync();
        
        return affectedRows > 0;
    }

    public async Task<int> Count<TEntity>() where TEntity : class
    {
        return await _dbContext.Set<TEntity>().CountAsync();
    }
}