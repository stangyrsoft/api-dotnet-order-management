using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories;

public class EfRepository<TEntity>(OrderDbContext context) : IRepository<TEntity>
    where TEntity : class
{
    protected OrderDbContext Context { get; } = context;
    protected DbSet<TEntity> Set => Context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Set.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Set.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        Set.Update(entity);
    }
}