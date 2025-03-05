using System.Linq.Expressions;

namespace Application.Interfaces.Repositories;

public interface IGenericRepository
{
    Task<IEnumerable<TEntity>> GetAsync<TEntity>(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "") where TEntity : class;

    Task<TEntity?> GetByIdAsync<TEntity>(object id) where TEntity : class;

    Task<TEntity?> GetFirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

    Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class;
    
    Task<int> InsertAsync<TEntity>(TEntity entity) where TEntity : class;

    Task AddMultipleEntityAsync<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class;

    Task DeleteAsync<TEntity>(object id) where TEntity : class;

    Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

    Task DeleteAsync<TEntity>(TEntity entityToDelete) where TEntity : class;
    
    Task UpdateAsync<TEntity>(TEntity entityToUpdate) where TEntity : class;
    
    Task UpdateMultipleEntityAsync<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class;
    
    Task<bool> RemoveMultipleEntityAsync<TEntity>(IEnumerable<TEntity> removeEntityList) where TEntity : class;

    Task<int> Count<TEntity>() where TEntity : class;
}