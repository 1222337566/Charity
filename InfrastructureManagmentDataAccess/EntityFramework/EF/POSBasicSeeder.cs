using InfrastrfuctureManagmentCore.Domains.Item;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public static class PosBasicSeeder
    {
        public static async Task SeedUnitsAndGroupsAsync(AppDbContext db)
        {
            if (!await db.Units.AnyAsync())
            {
                await db.Units.AddRangeAsync(
                    new Unit { Id = Guid.NewGuid(), UnitCode = "PCS", UnitNameAr = "قطعة", Symbol = "pcs" },
                    new Unit { Id = Guid.NewGuid(), UnitCode = "BOX", UnitNameAr = "كرتونة", Symbol = "box" },
                    new Unit { Id = Guid.NewGuid(), UnitCode = "M", UnitNameAr = "متر", Symbol = "m" },
                    new Unit { Id = Guid.NewGuid(), UnitCode = "TON", UnitNameAr = "طن", Symbol = "ton" }
                );
            }

            if (!await db.ItemGroups.AnyAsync())
            {
                await db.ItemGroups.AddRangeAsync(
                    new ItemGroup { Id = Guid.NewGuid(), GroupCode = "GR001", GroupNameAr = "مواد بناء" },
                    new ItemGroup { Id = Guid.NewGuid(), GroupCode = "GR002", GroupNameAr = "أدوات كهرباء" },
                    new ItemGroup { Id = Guid.NewGuid(), GroupCode = "GR003", GroupNameAr = "سباكة" },
                    new ItemGroup { Id = Guid.NewGuid(), GroupCode = "GR004", GroupNameAr = "خدمات" }
                );
            }

            await db.SaveChangesAsync();
        }
    }
}
