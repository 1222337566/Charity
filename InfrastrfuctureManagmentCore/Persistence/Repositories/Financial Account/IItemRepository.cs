using InfrastrfuctureManagmentCore.Domains.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account
{
    public interface IItemRepository
    {
        Task<List<Item>> GetAllAsync();
     
        Task<List<Item>> GetActiveAsync();
        Task<List<Item>> GetByOpticalTypeAsync(OpticalItemType type);
        Task<List<Item>> GetPrescriptionItemsAsync();
        Task<Item?> GetByIdAsync(Guid id);
        Task<Item?> GetByCodeAsync(string code);
        Task<Item?> GetByBarcodeAsync(string barcode);

        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);

        Task<bool> BarcodeExistsAsync(string barcode);
        Task<bool> BarcodeExistsAsync(string barcode, Guid excludeId);

        Task AddAsync(Item item);
        Task UpdateAsync(Item item);
    }
}
