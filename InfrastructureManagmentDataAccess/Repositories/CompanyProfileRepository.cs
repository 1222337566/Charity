using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Company;
using InfrastrfuctureManagmentCore.Persistence.Repositories.company;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
namespace InfrastructureManagmentDataAccess.Repositories
{


    public class CompanyProfileRepository : ICompanyProfileRepository
    {
        private readonly AppDbContext _db;

        public CompanyProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CompanyProfile?> GetActiveAsync()
        {
            return await _db.CompanyProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsActive);
        }

        public async Task<CompanyProfile?> GetByIdAsync(Guid id)
        {
            return await _db.CompanyProfiles
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(CompanyProfile profile)
        {
            await _db.CompanyProfiles.AddAsync(profile);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CompanyProfile profile)
        {
            _db.CompanyProfiles.Update(profile);
            await _db.SaveChangesAsync();
        }
    }
}
