using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentDataAccess.EntityFramework;

namespace InfrastructureManagmentCore.Persistence.Repositories.Ef;

public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _set;

    public EfRepository(AppDbContext db) { _db = db; _set = db.Set<T>(); }

    public Task<T?> GetByIdAsync(object id, CancellationToken ct = default) => _set.FindAsync(new[] { id }, ct).AsTask();
    public Task AddAsync(T entity, CancellationToken ct = default) => _set.AddAsync(entity, ct).AsTask();
    public void Update(T entity) => _set.Update(entity);
    public void Remove(T entity) => _set.Remove(entity);
    public IQueryable<T> Query() => _set.AsQueryable();
}