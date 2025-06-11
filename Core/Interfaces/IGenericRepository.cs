using Core.Entities;

namespace Core.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken);
    Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken);
    Task<TResult?> GetEntityWithSpecAsync<TResult>(ISpecification<T, TResult> spec, CancellationToken cancellationToken);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken);
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec, CancellationToken cancellationToken);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    bool Exists(int id);
    Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken);
}