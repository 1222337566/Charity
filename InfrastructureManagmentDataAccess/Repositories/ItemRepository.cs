using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
namespace InfrastructureManagmentDataAccess.Repositories
{


    

    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _db;

        public ItemRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Item>> GetAllAsync()
        {
            return await _db.Items
                .AsNoTracking()
                .Include(x => x.ItemGroup)
                .Include(x => x.Unit)
                .OrderBy(x => x.ItemCode)
                .ToListAsync();
        }
        public async Task<List<Item>> GetByOpticalTypeAsync(OpticalItemType type)
        {
            return await _db.Items
                .AsNoTracking()
                .Where(x => x.IsActive && x.OpticalItemType == type)
                .OrderBy(x => x.ItemCode)
                .ToListAsync();
        }

        public async Task<List<Item>> GetPrescriptionItemsAsync()
        {
            return await _db.Items
                .AsNoTracking()
                .Where(x => x.IsActive && x.RequiresPrescription)
                .OrderBy(x => x.ItemCode)
                .ToListAsync();
        }
        public async Task<List<Item>> GetActiveAsync()
        {
            return await _db.Items
                .AsNoTracking()
                .Include(x => x.ItemGroup)
                .Include(x => x.Unit)
                .Where(x => x.IsActive)
                .OrderBy(x => x.ItemCode)
                .ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(Guid id)
        {
            return await _db.Items
                .Include(x => x.ItemGroup)
                .Include(x => x.Unit)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Item?> GetByCodeAsync(string code)
        {
            return await _db.Items
                .FirstOrDefaultAsync(x => x.ItemCode == code);
        }

        public async Task<Item?> GetByBarcodeAsync(string barcode)
        {
            return await _db.Items
                .FirstOrDefaultAsync(x => x.Barcode == barcode);
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _db.Items.AnyAsync(x => x.ItemCode == code);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid excludeId)
        {
            return await _db.Items.AnyAsync(x => x.ItemCode == code && x.Id != excludeId);
        }

        public async Task<bool> BarcodeExistsAsync(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            return await _db.Items.AnyAsync(x => x.Barcode == barcode);
        }

        public async Task<bool> BarcodeExistsAsync(string barcode, Guid excludeId)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            return await _db.Items.AnyAsync(x => x.Barcode == barcode && x.Id != excludeId);
        }

        public async Task AddAsync(Item item)
        {
            await _db.Items.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Item item)
        {
            _db.Items.Update(item);
            await _db.SaveChangesAsync();
        }
    }
}
