namespace InfrastructureManagmentCore.Persistence.Repositories.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}