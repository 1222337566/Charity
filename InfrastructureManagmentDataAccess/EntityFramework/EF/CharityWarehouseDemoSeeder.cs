using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public sealed class CharityWarehouseSeedResult
    {
        public int WarehousesCreated { get; set; }
        public int ItemsCreated { get; set; }
        public int OpeningEntriesCreated { get; set; }
        public int ReceiptVouchersCreated { get; set; }
        public int IssueVouchersCreated { get; set; }
        public int NeedRequestsCreated { get; set; }
        public int ReturnVouchersCreated { get; set; }
        public int DisposalVouchersCreated { get; set; }
        public int TransferOperationsCreated { get; set; }
        public int StockBalanceRowsAffected { get; set; }
        public int StockTransactionsCreated { get; set; }
    }

    public static class CharityWarehouseDemoSeeder
    {
        public static async Task<CharityWarehouseSeedResult> SeedAsync(AppDbContext db, string? createdByUserId = null)
        {
            var result = new CharityWarehouseSeedResult();

            await using var tx = await db.Database.BeginTransactionAsync();

            var boxUnit = await EnsureUnitAsync(db, "BOX", "كرتونة", "box");
            var pcsUnit = await EnsureUnitAsync(db, "PCS", "قطعة", "pcs");
            var kitUnit = await EnsureUnitAsync(db, "KIT", "طقم", "kit");

            var foodGroup = await EnsureItemGroupAsync(db, "CH-GR-FOOD", "مواد غذائية");
            var reliefGroup = await EnsureItemGroupAsync(db, "CH-GR-RELIEF", "إغاثة وكساء");
            var medicalGroup = await EnsureItemGroupAsync(db, "CH-GR-MED", "مستلزمات طبية");
            var educationGroup = await EnsureItemGroupAsync(db, "CH-GR-EDU", "دعم تعليمي");
            var hygieneGroup = await EnsureItemGroupAsync(db, "CH-GR-HYG", "نظافة شخصية");

            await db.SaveChangesAsync();

            var whMainResult = await EnsureWarehouseAsync(db, "WH-CH-01", "المخزن الرئيسي للجمعية", "Main Charity Store", true, "المقر الرئيسي");
            var whReliefResult = await EnsureWarehouseAsync(db, "WH-CH-02", "مخزن الإغاثة", "Relief Store", false, "فرع الإعانات");
            var whMedicalResult = await EnsureWarehouseAsync(db, "WH-CH-03", "المخزن الطبي", "Medical Store", false, "مخزن المستلزمات الطبية");

            var whMain = whMainResult.Entity;
            var whRelief = whReliefResult.Entity;
            var whMedical = whMedicalResult.Entity;
            result.WarehousesCreated += (whMainResult.Created ? 1 : 0) + (whReliefResult.Created ? 1 : 0) + (whMedicalResult.Created ? 1 : 0);

            var itemFoodBoxResult = await EnsureItemAsync(db, "ITM-CH-001", "كرتونة مواد غذائية", foodGroup.Id, boxUnit.Id, 210m, 250m, 15m, 30m,
                description: "صنف تجريبي للمساعدات الغذائية");
            var itemBlanketResult = await EnsureItemAsync(db, "ITM-CH-002", "بطانية شتوية", reliefGroup.Id, pcsUnit.Id, 150m, 180m, 10m, 20m,
                description: "صنف تجريبي للإعانات الموسمية");
            var itemMedKitResult = await EnsureItemAsync(db, "ITM-CH-003", "عبوة دواء مزمن", medicalGroup.Id, pcsUnit.Id, 80m, 95m, 20m, 40m,
                description: "عبوة علاج شهرية للمستفيدين");
            var itemSchoolBagResult = await EnsureItemAsync(db, "ITM-CH-004", "حقيبة مدرسية كاملة", educationGroup.Id, pcsUnit.Id, 120m, 140m, 8m, 15m,
                description: "حقيبة تشمل الأدوات الأساسية");
            var itemWaterResult = await EnsureItemAsync(db, "ITM-CH-005", "كرتونة مياه", foodGroup.Id, boxUnit.Id, 70m, 90m, 12m, 24m,
                description: "مياه للشرب في القوافل");
            var itemWheelchairResult = await EnsureItemAsync(db, "ITM-CH-006", "كرسي متحرك", medicalGroup.Id, pcsUnit.Id, 2000m, 2200m, 1m, 2m,
                description: "مساعدة للأسر الأكثر احتياجًا");
            var itemHygieneResult = await EnsureItemAsync(db, "ITM-CH-007", "شنطة نظافة شخصية", hygieneGroup.Id, kitUnit.Id, 60m, 75m, 10m, 20m,
                description: "طقم نظافة للمستفيدين");

            var itemFoodBox = itemFoodBoxResult.Entity;
            var itemBlanket = itemBlanketResult.Entity;
            var itemMedKit = itemMedKitResult.Entity;
            var itemSchoolBag = itemSchoolBagResult.Entity;
            var itemWater = itemWaterResult.Entity;
            var itemWheelchair = itemWheelchairResult.Entity;
            var itemHygiene = itemHygieneResult.Entity;
            result.ItemsCreated += (itemFoodBoxResult.Created ? 1 : 0) + (itemBlanketResult.Created ? 1 : 0) + (itemMedKitResult.Created ? 1 : 0)
                + (itemSchoolBagResult.Created ? 1 : 0) + (itemWaterResult.Created ? 1 : 0) + (itemWheelchairResult.Created ? 1 : 0) + (itemHygieneResult.Created ? 1 : 0);

            await db.SaveChangesAsync();

            var beneficiaries = await db.Set<Beneficiary>()
                .OrderBy(x => x.Code)
                .Take(3)
                .ToListAsync();

            var projects = await db.Set<CharityProject>()
                .OrderBy(x => x.Code)
                .Take(2)
                .ToListAsync();

            var firstBeneficiary = beneficiaries.ElementAtOrDefault(0);
            var secondBeneficiary = beneficiaries.ElementAtOrDefault(1) ?? firstBeneficiary;
            var firstProject = projects.ElementAtOrDefault(0);
            var secondProject = projects.ElementAtOrDefault(1) ?? firstProject;

            var demoItemIds = new[]
            {
                itemFoodBox.Id, itemBlanket.Id, itemMedKit.Id, itemSchoolBag.Id, itemWater.Id, itemWheelchair.Id, itemHygiene.Id
            };
            var demoWarehouseIds = new[] { whMain.Id, whRelief.Id, whMedical.Id };

            var balances = await db.Set<ItemWarehouseBalance>()
                .Where(x => demoItemIds.Contains(x.ItemId) && demoWarehouseIds.Contains(x.WarehouseId))
                .ToListAsync();

            var balanceMap = balances.ToDictionary(x => (x.ItemId, x.WarehouseId));
            var newBalances = new List<ItemWarehouseBalance>();
            var newTransactions = new List<StockTransaction>();

            ItemWarehouseBalance GetOrCreateBalance(Guid itemId, Guid warehouseId)
            {
                if (balanceMap.TryGetValue((itemId, warehouseId), out var balance))
                    return balance;

                balance = new ItemWarehouseBalance
                {
                    Id = Guid.NewGuid(),
                    ItemId = itemId,
                    WarehouseId = warehouseId,
                    QuantityOnHand = 0,
                    ReservedQuantity = 0,
                    AvailableQuantity = 0,
                    LastUpdatedUtc = DateTime.UtcNow
                };

                balanceMap[(itemId, warehouseId)] = balance;
                newBalances.Add(balance);
                return balance;
            }

            void Increase(Guid itemId, Guid warehouseId, decimal quantity, decimal unitCost, StockTransactionType type, DateTime date, string refType, string refNo, string? notes = null, Guid? relatedWarehouseId = null)
            {
                var balance = GetOrCreateBalance(itemId, warehouseId);
                balance.QuantityOnHand += quantity;
                balance.AvailableQuantity = balance.QuantityOnHand - balance.ReservedQuantity;
                balance.LastUpdatedUtc = DateTime.UtcNow;

                newTransactions.Add(new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ItemId = itemId,
                    WarehouseId = warehouseId,
                    RelatedWarehouseId = relatedWarehouseId,
                    TransactionType = type,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    TransactionDateUtc = date,
                    ReferenceType = refType,
                    ReferenceNumber = refNo,
                    Notes = notes,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            void Decrease(Guid itemId, Guid warehouseId, decimal quantity, decimal unitCost, StockTransactionType type, DateTime date, string refType, string refNo, string? notes = null, Guid? relatedWarehouseId = null)
            {
                var balance = GetOrCreateBalance(itemId, warehouseId);
                if (balance.AvailableQuantity < quantity)
                    throw new InvalidOperationException($"الرصيد غير كافٍ للصنف {itemId} داخل المخزن {warehouseId} أثناء توليد الديمو.");

                balance.QuantityOnHand -= quantity;
                balance.AvailableQuantity = balance.QuantityOnHand - balance.ReservedQuantity;
                balance.LastUpdatedUtc = DateTime.UtcNow;

                newTransactions.Add(new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ItemId = itemId,
                    WarehouseId = warehouseId,
                    RelatedWarehouseId = relatedWarehouseId,
                    TransactionType = type,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    TransactionDateUtc = date,
                    ReferenceType = refType,
                    ReferenceNumber = refNo,
                    Notes = notes,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            async Task<bool> HasStockReferenceAsync(string referenceNumber) =>
                await db.Set<StockTransaction>().AnyAsync(x => x.ReferenceNumber == referenceNumber);

            if (!await HasStockReferenceAsync("OB-CH-0001"))
            {
                Increase(itemFoodBox.Id, whMain.Id, 100m, 210m, StockTransactionType.OpeningBalance, new DateTime(2026, 1, 1), "OpeningBalance", "OB-CH-0001", "رصيد افتتاحي غذائي");
                Increase(itemBlanket.Id, whMain.Id, 80m, 150m, StockTransactionType.OpeningBalance, new DateTime(2026, 1, 1), "OpeningBalance", "OB-CH-0001", "رصيد افتتاحي بطاطين");
                Increase(itemSchoolBag.Id, whMain.Id, 60m, 120m, StockTransactionType.OpeningBalance, new DateTime(2026, 1, 1), "OpeningBalance", "OB-CH-0001", "رصيد افتتاحي تعليم");
                Increase(itemWater.Id, whRelief.Id, 70m, 70m, StockTransactionType.OpeningBalance, new DateTime(2026, 1, 1), "OpeningBalance", "OB-CH-0001", "رصيد افتتاحي مياه");
                Increase(itemHygiene.Id, whRelief.Id, 50m, 60m, StockTransactionType.OpeningBalance, new DateTime(2026, 1, 1), "OpeningBalance", "OB-CH-0001", "رصيد افتتاحي شنط نظافة");
                Increase(itemMedKit.Id, whMedical.Id, 120m, 80m, StockTransactionType.OpeningBalance, new DateTime(2026, 1, 1), "OpeningBalance", "OB-CH-0001", "رصيد افتتاحي طبي");
                Increase(itemWheelchair.Id, whMedical.Id, 8m, 2000m, StockTransactionType.OpeningBalance, new DateTime(2026, 1, 1), "OpeningBalance", "OB-CH-0001", "رصيد افتتاحي كراسي متحركة");
                result.OpeningEntriesCreated = 1;
            }

            if (!await db.Set<CharityStoreReceipt>().AnyAsync(x => x.ReceiptNumber == "RCV-CH-0001"))
            {
                var receipt = new CharityStoreReceipt
                {
                    Id = Guid.NewGuid(),
                    ReceiptNumber = "RCV-CH-0001",
                    WarehouseId = whMain.Id,
                    ReceiptDate = new DateTime(2026, 1, 15),
                    SourceType = "Donation",
                    SourceName = "تبرع عيني موسمي",
                    Notes = "إضافة تجريبية للمخزن الرئيسي",
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedByUserId = createdByUserId,
                    Lines = new List<CharityStoreReceiptLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemFoodBox.Id, Quantity = 40m, UnitCost = 215m, Notes = "كميات إضافية" },
                        new() { Id = Guid.NewGuid(), ItemId = itemSchoolBag.Id, Quantity = 30m, UnitCost = 122m, Notes = "دعم تعليمي" }
                    }
                };

                await db.Set<CharityStoreReceipt>().AddAsync(receipt);
                Increase(itemFoodBox.Id, whMain.Id, 40m, 215m, StockTransactionType.Purchase, receipt.ReceiptDate, "CharityStoreReceipt", receipt.ReceiptNumber, receipt.Notes);
                Increase(itemSchoolBag.Id, whMain.Id, 30m, 122m, StockTransactionType.Purchase, receipt.ReceiptDate, "CharityStoreReceipt", receipt.ReceiptNumber, receipt.Notes);
                result.ReceiptVouchersCreated++;
            }

            if (!await db.Set<CharityStoreReceipt>().AnyAsync(x => x.ReceiptNumber == "RCV-CH-0002"))
            {
                var receipt = new CharityStoreReceipt
                {
                    Id = Guid.NewGuid(),
                    ReceiptNumber = "RCV-CH-0002",
                    WarehouseId = whRelief.Id,
                    ReceiptDate = new DateTime(2026, 1, 20),
                    SourceType = "Purchase",
                    SourceName = "تعزيز مخزون الإغاثة",
                    Notes = "إضافة بطاطين ومياه",
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedByUserId = createdByUserId,
                    Lines = new List<CharityStoreReceiptLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemBlanket.Id, Quantity = 25m, UnitCost = 155m },
                        new() { Id = Guid.NewGuid(), ItemId = itemWater.Id, Quantity = 40m, UnitCost = 72m }
                    }
                };

                await db.Set<CharityStoreReceipt>().AddAsync(receipt);
                Increase(itemBlanket.Id, whRelief.Id, 25m, 155m, StockTransactionType.Purchase, receipt.ReceiptDate, "CharityStoreReceipt", receipt.ReceiptNumber, receipt.Notes);
                Increase(itemWater.Id, whRelief.Id, 40m, 72m, StockTransactionType.Purchase, receipt.ReceiptDate, "CharityStoreReceipt", receipt.ReceiptNumber, receipt.Notes);
                result.ReceiptVouchersCreated++;
            }

            if (!await db.Set<CharityStoreReceipt>().AnyAsync(x => x.ReceiptNumber == "RCV-CH-0003"))
            {
                var receipt = new CharityStoreReceipt
                {
                    Id = Guid.NewGuid(),
                    ReceiptNumber = "RCV-CH-0003",
                    WarehouseId = whMedical.Id,
                    ReceiptDate = new DateTime(2026, 1, 25),
                    SourceType = "Purchase",
                    SourceName = "دعم طبي",
                    Notes = "أدوية وكراسي متحركة",
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedByUserId = createdByUserId,
                    Lines = new List<CharityStoreReceiptLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemMedKit.Id, Quantity = 50m, UnitCost = 82m, BatchNo = "MED-2026-A" },
                        new() { Id = Guid.NewGuid(), ItemId = itemWheelchair.Id, Quantity = 2m, UnitCost = 2050m }
                    }
                };

                await db.Set<CharityStoreReceipt>().AddAsync(receipt);
                Increase(itemMedKit.Id, whMedical.Id, 50m, 82m, StockTransactionType.Purchase, receipt.ReceiptDate, "CharityStoreReceipt", receipt.ReceiptNumber, receipt.Notes);
                Increase(itemWheelchair.Id, whMedical.Id, 2m, 2050m, StockTransactionType.Purchase, receipt.ReceiptDate, "CharityStoreReceipt", receipt.ReceiptNumber, receipt.Notes);
                result.ReceiptVouchersCreated++;
            }

            if (!await db.Set<CharityStoreIssue>().AnyAsync(x => x.IssueNumber == "ISS-CH-0001"))
            {
                var issue = new CharityStoreIssue
                {
                    Id = Guid.NewGuid(),
                    IssueNumber = "ISS-CH-0001",
                    WarehouseId = whRelief.Id,
                    IssueDate = new DateTime(2026, 2, 1),
                    IssueType = "Beneficiary",
                    BeneficiaryId = firstBeneficiary?.Id,
                    IssuedToName = firstBeneficiary?.FullName ?? "مستفيد تجريبي",
                    Notes = "صرف إعانة مباشرة للمستفيد",
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedByUserId = createdByUserId,
                    Lines = new List<CharityStoreIssueLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemBlanket.Id, Quantity = 4m, UnitCost = 155m },
                        new() { Id = Guid.NewGuid(), ItemId = itemWater.Id, Quantity = 6m, UnitCost = 72m }
                    }
                };

                await db.Set<CharityStoreIssue>().AddAsync(issue);
                Decrease(itemBlanket.Id, whRelief.Id, 4m, 155m, StockTransactionType.Sale, issue.IssueDate, "CharityStoreIssue", issue.IssueNumber, issue.Notes);
                Decrease(itemWater.Id, whRelief.Id, 6m, 72m, StockTransactionType.Sale, issue.IssueDate, "CharityStoreIssue", issue.IssueNumber, issue.Notes);
                result.IssueVouchersCreated++;
            }

            if (!await db.Set<CharityStoreIssue>().AnyAsync(x => x.IssueNumber == "ISS-CH-0002"))
            {
                var issue = new CharityStoreIssue
                {
                    Id = Guid.NewGuid(),
                    IssueNumber = "ISS-CH-0002",
                    WarehouseId = whMain.Id,
                    IssueDate = new DateTime(2026, 2, 5),
                    IssueType = "Project",
                    ProjectId = firstProject?.Id,
                    IssuedToName = firstProject?.Name ?? "مشروع تجريبي",
                    Notes = "صرف لصالح مشروع ميداني",
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedByUserId = createdByUserId,
                    Lines = new List<CharityStoreIssueLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemFoodBox.Id, Quantity = 20m, UnitCost = 215m },
                        new() { Id = Guid.NewGuid(), ItemId = itemSchoolBag.Id, Quantity = 15m, UnitCost = 122m }
                    }
                };

                await db.Set<CharityStoreIssue>().AddAsync(issue);
                Decrease(itemFoodBox.Id, whMain.Id, 20m, 215m, StockTransactionType.Sale, issue.IssueDate, "CharityStoreIssue", issue.IssueNumber, issue.Notes);
                Decrease(itemSchoolBag.Id, whMain.Id, 15m, 122m, StockTransactionType.Sale, issue.IssueDate, "CharityStoreIssue", issue.IssueNumber, issue.Notes);
                result.IssueVouchersCreated++;
            }

            if (!await db.Set<CharityStoreIssue>().AnyAsync(x => x.IssueNumber == "ISS-CH-0003"))
            {
                var issue = new CharityStoreIssue
                {
                    Id = Guid.NewGuid(),
                    IssueNumber = "ISS-CH-0003",
                    WarehouseId = whMedical.Id,
                    IssueDate = new DateTime(2026, 2, 10),
                    IssueType = "Beneficiary",
                    BeneficiaryId = secondBeneficiary?.Id,
                    IssuedToName = secondBeneficiary?.FullName ?? "مستفيد طبي",
                    Notes = "صرف مستلزمات طبية",
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedByUserId = createdByUserId,
                    Lines = new List<CharityStoreIssueLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemMedKit.Id, Quantity = 8m, UnitCost = 82m },
                        new() { Id = Guid.NewGuid(), ItemId = itemWheelchair.Id, Quantity = 1m, UnitCost = 2050m }
                    }
                };

                await db.Set<CharityStoreIssue>().AddAsync(issue);
                Decrease(itemMedKit.Id, whMedical.Id, 8m, 82m, StockTransactionType.Sale, issue.IssueDate, "CharityStoreIssue", issue.IssueNumber, issue.Notes);
                Decrease(itemWheelchair.Id, whMedical.Id, 1m, 2050m, StockTransactionType.Sale, issue.IssueDate, "CharityStoreIssue", issue.IssueNumber, issue.Notes);
                result.IssueVouchersCreated++;
            }

            if (!await db.Set<StockNeedRequest>().AnyAsync(x => x.RequestNumber == "NREQ-CH-0001"))
            {
                await db.Set<StockNeedRequest>().AddAsync(new StockNeedRequest
                {
                    Id = Guid.NewGuid(),
                    RequestNumber = "NREQ-CH-0001",
                    RequestDate = new DateTime(2026, 2, 12),
                    RequestType = "Project",
                    ProjectId = firstProject?.Id,
                    RequestedByName = "منسق المشروع",
                    Status = "Approved",
                    Notes = "طلب احتياج تشغيلي للمشروع",
                    CreatedAtUtc = DateTime.UtcNow,
                    Lines = new List<StockNeedRequestLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemFoodBox.Id, RequestedQuantity = 15m, ApprovedQuantity = 15m, FulfilledQuantity = 10m },
                        new() { Id = Guid.NewGuid(), ItemId = itemSchoolBag.Id, RequestedQuantity = 20m, ApprovedQuantity = 18m, FulfilledQuantity = 10m }
                    }
                });
                result.NeedRequestsCreated++;
            }

            if (!await db.Set<StockNeedRequest>().AnyAsync(x => x.RequestNumber == "NREQ-CH-0002"))
            {
                await db.Set<StockNeedRequest>().AddAsync(new StockNeedRequest
                {
                    Id = Guid.NewGuid(),
                    RequestNumber = "NREQ-CH-0002",
                    RequestDate = new DateTime(2026, 2, 15),
                    RequestType = "Beneficiary",
                    BeneficiaryId = secondBeneficiary?.Id,
                    RequestedByName = "باحث اجتماعي",
                    Status = "Pending",
                    Notes = "طلب قيد المراجعة",
                    CreatedAtUtc = DateTime.UtcNow,
                    Lines = new List<StockNeedRequestLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemBlanket.Id, RequestedQuantity = 2m, ApprovedQuantity = 0m, FulfilledQuantity = 0m },
                        new() { Id = Guid.NewGuid(), ItemId = itemMedKit.Id, RequestedQuantity = 4m, ApprovedQuantity = 0m, FulfilledQuantity = 0m }
                    }
                });
                result.NeedRequestsCreated++;
            }

            if (!await db.Set<StockReturnVoucher>().AnyAsync(x => x.VoucherNumber == "RTV-CH-0001"))
            {
                var voucher = new StockReturnVoucher
                {
                    Id = Guid.NewGuid(),
                    VoucherNumber = "RTV-CH-0001",
                    VoucherDate = new DateTime(2026, 2, 18),
                    WarehouseId = whRelief.Id,
                    ReturnType = "Project",
                    ProjectId = firstProject?.Id,
                    Reason = "فائض لم يُستخدم",
                    Notes = "مرتجع من نشاط ميداني",
                    CreatedAtUtc = DateTime.UtcNow,
                    Lines = new List<StockReturnVoucherLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemWater.Id, Quantity = 2m, UnitCost = 72m },
                        new() { Id = Guid.NewGuid(), ItemId = itemBlanket.Id, Quantity = 1m, UnitCost = 155m }
                    }
                };

                await db.Set<StockReturnVoucher>().AddAsync(voucher);
                Increase(itemWater.Id, whRelief.Id, 2m, 72m, StockTransactionType.SaleReturn, voucher.VoucherDate, "StockReturnVoucher", voucher.VoucherNumber, voucher.Notes);
                Increase(itemBlanket.Id, whRelief.Id, 1m, 155m, StockTransactionType.SaleReturn, voucher.VoucherDate, "StockReturnVoucher", voucher.VoucherNumber, voucher.Notes);
                result.ReturnVouchersCreated++;
            }

            if (!await db.Set<StockDisposalVoucher>().AnyAsync(x => x.VoucherNumber == "DSP-CH-0001"))
            {
                var voucher = new StockDisposalVoucher
                {
                    Id = Guid.NewGuid(),
                    VoucherNumber = "DSP-CH-0001",
                    VoucherDate = new DateTime(2026, 2, 20),
                    WarehouseId = whMedical.Id,
                    DisposalType = "Expiry",
                    Reason = "انتهاء صلاحية جزء من العبوات",
                    Notes = "إعدام تجريبي",
                    CreatedAtUtc = DateTime.UtcNow,
                    Lines = new List<StockDisposalVoucherLine>
                    {
                        new() { Id = Guid.NewGuid(), ItemId = itemMedKit.Id, Quantity = 3m, UnitCost = 82m }
                    }
                };

                await db.Set<StockDisposalVoucher>().AddAsync(voucher);
                Decrease(itemMedKit.Id, whMedical.Id, 3m, 82m, StockTransactionType.AdjustmentDecrease, voucher.VoucherDate, "StockDisposalVoucher", voucher.VoucherNumber, voucher.Notes);
                result.DisposalVouchersCreated++;
            }

            if (!await HasStockReferenceAsync("TRF-CH-0001"))
            {
                var transferDate = new DateTime(2026, 2, 22);
                Decrease(itemFoodBox.Id, whMain.Id, 10m, 215m, StockTransactionType.TransferOut, transferDate, "StockTransfer", "TRF-CH-0001", "تحويل للمخزن الإغاثي", whRelief.Id);
                Increase(itemFoodBox.Id, whRelief.Id, 10m, 215m, StockTransactionType.TransferIn, transferDate, "StockTransfer", "TRF-CH-0001", "تحويل وارد من المخزن الرئيسي", whMain.Id);
                result.TransferOperationsCreated++;
            }

            if (!await HasStockReferenceAsync("TRF-CH-0002"))
            {
                var transferDate = new DateTime(2026, 2, 24);
                Decrease(itemSchoolBag.Id, whMain.Id, 5m, 122m, StockTransactionType.TransferOut, transferDate, "StockTransfer", "TRF-CH-0002", "تحويل للمخزن الطبي لصالح لجنة الحالات", whMedical.Id);
                Increase(itemSchoolBag.Id, whMedical.Id, 5m, 122m, StockTransactionType.TransferIn, transferDate, "StockTransfer", "TRF-CH-0002", "تحويل وارد من المخزن الرئيسي", whMain.Id);
                result.TransferOperationsCreated++;
            }

            if (newBalances.Count > 0)
                await db.Set<ItemWarehouseBalance>().AddRangeAsync(newBalances);

            if (newTransactions.Count > 0)
                await db.Set<StockTransaction>().AddRangeAsync(newTransactions);

            result.StockBalanceRowsAffected = balanceMap.Count(x => demoItemIds.Contains(x.Key.Item1) && demoWarehouseIds.Contains(x.Key.Item2));
            result.StockTransactionsCreated = newTransactions.Count;

            await db.SaveChangesAsync();
            await tx.CommitAsync();

            return result;
        }

        private static async Task<Unit> EnsureUnitAsync(AppDbContext db, string code, string nameAr, string symbol)
        {
            var entity = await db.Set<Unit>().FirstOrDefaultAsync(x => x.UnitCode == code);
            if (entity != null)
                return entity;

            entity = new Unit
            {
                Id = Guid.NewGuid(),
                UnitCode = code,
                UnitNameAr = nameAr,
                Symbol = symbol,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await db.Set<Unit>().AddAsync(entity);
            return entity;
        }

        private static async Task<ItemGroup> EnsureItemGroupAsync(AppDbContext db, string code, string nameAr)
        {
            var entity = await db.Set<ItemGroup>().FirstOrDefaultAsync(x => x.GroupCode == code);
            if (entity != null)
                return entity;

            entity = new ItemGroup
            {
                Id = Guid.NewGuid(),
                GroupCode = code,
                GroupNameAr = nameAr,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await db.Set<ItemGroup>().AddAsync(entity);
            return entity;
        }

        private static async Task<(Warehouse Entity, bool Created)> EnsureWarehouseAsync(AppDbContext db, string code, string nameAr, string? nameEn, bool isMain, string? address)
        {
            var entity = await db.Set<Warehouse>().FirstOrDefaultAsync(x => x.WarehouseCode == code);
            if (entity != null)
                return (entity, false);

            entity = new Warehouse
            {
                Id = Guid.NewGuid(),
                WarehouseCode = code,
                WarehouseNameAr = nameAr,
                WarehouseNameEn = nameEn,
                Address = address,
                Notes = "تم إنشاؤه تلقائيًا ضمن بيانات الديمو للمخازن",
                IsMain = isMain,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await db.Set<Warehouse>().AddAsync(entity);
            return (entity, true);
        }

        private static async Task<(Item Entity, bool Created)> EnsureItemAsync(
            AppDbContext db,
            string code,
            string nameAr,
            Guid groupId,
            Guid unitId,
            decimal purchasePrice,
            decimal salePrice,
            decimal minimumQuantity,
            decimal reorderQuantity,
            string? description = null)
        {
            var entity = await db.Set<Item>().FirstOrDefaultAsync(x => x.ItemCode == code);
            if (entity != null)
                return (entity, false);

            entity = new Item
            {
                Id = Guid.NewGuid(),
                ItemCode = code,
                ItemNameAr = nameAr,
                ItemGroupId = groupId,
                UnitId = unitId,
                IsService = false,
                IsStockItem = true,
                IsActive = true,
                PurchasePrice = purchasePrice,
                SalePrice = salePrice,
                MinimumQuantity = minimumQuantity,
                ReorderQuantity = reorderQuantity,
                IsTaxable = false,
                TaxRate = 0m,
                Description = description,
                CreatedAtUtc = DateTime.UtcNow,
                OpticalItemType = OpticalItemType.General
            };

            await db.Set<Item>().AddAsync(entity);
            return (entity, true);
        }
    }
}
