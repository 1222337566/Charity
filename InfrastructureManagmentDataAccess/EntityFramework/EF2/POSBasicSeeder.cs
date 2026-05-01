using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
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
        public static async Task SeedBeneficiaryCategoriesAsync(AppDbContext db)
        {
            var items = new[]
            {
        new { Code = "Women", NameAr = "سيدات", IsWaiting = false, RequiresDisability = false },
        new { Code = "Men", NameAr = "ذكور", IsWaiting = false, RequiresDisability = false },
        new { Code = "Disability", NameAr = "ذوي إعاقة", IsWaiting = false, RequiresDisability = true },

        new { Code = "WaitingWomen", NameAr = "قائمة انتظار سيدات", IsWaiting = true, RequiresDisability = false },
        new { Code = "WaitingMen", NameAr = "قائمة انتظار ذكور", IsWaiting = true, RequiresDisability = false },
        new { Code = "WaitingDisability", NameAr = "قائمة انتظار ذوي إعاقة", IsWaiting = true, RequiresDisability = true },

        new { Code = "ProjectBeneficiary", NameAr = "مستفيد مشروع", IsWaiting = false, RequiresDisability = false },
        new { Code = "ActivityParticipant", NameAr = "مشارك في نشاط", IsWaiting = false, RequiresDisability = false },
        new { Code = "ServiceBeneficiary", NameAr = "منتفع من خدمة", IsWaiting = false, RequiresDisability = false },
        new { Code = "ServiceReceived", NameAr = "تم تلقي الخدمة", IsWaiting = false, RequiresDisability = false },
    };

            var sort = 1;

            foreach (var item in items)
            {
                var exists = await db.Set<BeneficiaryCategory>()
                    .AnyAsync(x => x.Code == item.Code);

                if (exists)
                    continue;

                db.Set<BeneficiaryCategory>().Add(new BeneficiaryCategory
                {
                    Id = Guid.NewGuid(),
                    Code = item.Code,
                    NameAr = item.NameAr,
                    IsWaitingListCategory = item.IsWaiting,
                    RequiresDisabilityType = item.RequiresDisability,
                    IsActive = true,
                    SortOrder = sort++,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
        }

    }
}
