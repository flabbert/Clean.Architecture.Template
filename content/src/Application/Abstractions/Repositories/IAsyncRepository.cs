using Clean.Architecture.Template.SharedKernel;

namespace Clean.Architecture.Template.Application.Abstractions.Repositories;

public interface IAsyncRepository<T, in TX>
    where T : IIdentifiable<TX>
    where TX : IEquatable<TX>
{
    ValueTask<T?> GetByIdAsync(TX id, CancellationToken cancellationToken = default);
    ValueTask<T?> GetByIdAsync(TX id, bool splitQuery = false, bool trackQuery = true, CancellationToken cancellationToken = default);
    IQueryable<T> GetAsQueryable();
    IQueryable<T> GetAsQueryable(bool split, bool tracked = false);
    ValueTask<List<T>> ListAllAsync(CancellationToken cancellationToken = default);
    ValueTask<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TX id, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyList<T>> GetPagedResponseAsync(int page, int size, CancellationToken cancellationToken = default);
}