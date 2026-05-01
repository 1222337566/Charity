using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly AppDbContext _db;

    public WarehouseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Warehouse>> GetAllAsync()
    {
        return await _db.Warehouses
            .AsNoTracking()
            .OrderBy(x => x.WarehouseCode)
            .ToListAsync();
    }

    public async Task<List<Warehouse>> GetActiveAsync()
    {
        return await _db.Warehouses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.WarehouseCode)
            .ToListAsync();
    }

    public async Task<Warehouse?> GetByIdAsync(Guid id)
    {
        return await _db.Warehouses
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Warehouse?> GetByCodeAsync(string code)
    {
        return await _db.Warehouses
            .FirstOrDefaultAsync(x => x.WarehouseCode == code);
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _db.Warehouses
            .AnyAsync(x => x.WarehouseCode == code);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid excludeId)
    {
        return await _db.Warehouses
            .AnyAsync(x => x.WarehouseCode == code && x.Id != excludeId);
    }

    public async Task AddAsync(Warehouse warehouse)
    {
        await _db.Warehouses.AddAsync(warehouse);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Warehouse warehouse)
    {
        _db.Warehouses.Update(warehouse);
        await _db.SaveChangesAsync();
    }
}