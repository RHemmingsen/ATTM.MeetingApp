using ATTM.MeetingApp.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ATTM.MeetingApp.Persistence.Repositories;

public class BaseRepository<T>(MeetingDbContext dbContext) : IAsyncRepository<T> where T : class
{
    protected readonly MeetingDbContext _dbContext = dbContext;

    public virtual async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Set<T>().FindAsync([id], cancellationToken);

    public virtual async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
