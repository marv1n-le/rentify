using Rentify.BusinessObjects.Entities.Base;
using System.Linq.Expressions;

namespace Rentify.Repositories.Implement;

public interface IGenericRepository<T> where T : BaseEntity
{
    IEnumerable<T> Get(int index, int pageSize);
    T GetById(object id);
    void Insert(T obj);
    void InsertRange(List<T> obj);
    Task InsertCollection(ICollection<T> collection);
    void Update(T obj);
    void Delete(object id);
    Task<IEnumerable<T>> GetAsync(int index, int pageSize);
    Task<T> GetByIdAsync(object id);
    Task<T?> GetByIdNotDeletedAsync(object id);
    List<T> GetAll();
    Task<IEnumerable<T>> GetAllAsync();
    Task InsertAsync(T obj);
    Task InsertWithoutAuditAsync(T obj);
    Task UpdateAsync(T obj);
    Task DeleteAsync(object id);
    Task DeleteAsync(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task SoftDeleteAsync(object id);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IQueryable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
    Task<IQueryable<T>> GetAllQueryableAsync();
    Task<List<T>> FindListAsync(Expression<Func<T, bool>> predicate);
    Task<bool> IsEntityExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
}