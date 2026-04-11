using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Infrastructure.Persistence;
using Clean.Architecture.Template.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Clean.Architecture.Template.Infrastructure.Repositories;

public class BaseRepository<T, TX>(ApplicationDbContext dbContext) : IAsyncRepository<T, TX>
    where T : class, IIdentifiable<TX>
    where TX : IEquatable<TX>
{
    protected readonly ApplicationDbContext DbContext = dbContext;

    public async ValueTask<T?> GetByIdAsync(TX id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().FindAsync([id], cancellationToken);
    }

    public async ValueTask<T?> GetByIdAsync(TX id, bool splitQuery = false, bool trackQuery = true, CancellationToken cancellationToken = default)
    {
        var query = DbContext.Set<T>().AsQueryable();

        if (splitQuery)
            query = query.AsSplitQuery();

        if (!trackQuery)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public IQueryable<T> GetAsQueryable()
    {
        return DbContext.Set<T>().AsQueryable();
    }

    public IQueryable<T> GetAsQueryable(bool split, bool tracked = false)
    {
        var query = DbContext.Set<T>().AsQueryable();

        if (split)
            query = query.AsSplitQuery();

        if (!tracked)
            query = query.AsNoTracking();

        return query;
    }

    public async ValueTask<List<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async ValueTask<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async ValueTask UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DeleteAsync(TX id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
        {
            DbContext.Set<T>().Remove(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async ValueTask<IReadOnlyList<T>> GetPagedResponseAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>()
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
