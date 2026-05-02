using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Optics;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Domains.Prescriptions;
using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Domains.SalesReturn;
using InfrastrfuctureManagmentCore.Domains.Suppliers;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

// TODO: عدّل الـusing statements حسب namespaces الفعلية في مشروعك
// using InfrastructureManagmentDataAccess.EntityFramework;
// using InfrastructureManagmentCore.Domains.Optics;
// using InfrastructureManagmentCore.Domains.Inventory;
// using InfrastructureManagmentCore.Domains.Finance;
// using InfrastructureManagmentCore.Domains.Sales;
// using InfrastructureManagmentCore.Domains.Purchases;

namespace InfrastructureManagmentWeb.Seeding
{
    /// <summary>
    /// Demo data مخصوصة لموديول محل النظارات.
    /// ملاحظات مهمة:
    /// 1) الكلاس يفترض إن الكيانات دي موجودة بالفعل بنفس الأسماء التي اتفقنا عليها في الشات.
    /// 2) لو عندك namespace مختلف أو property names مختلفة، عدّلها فقط من أعلى الملف.
    /// 3) الكلاس يزرع البيانات مباشرة داخل الجداول. لو عندك StockTransactions / JournalEntries / Services إلزامية،
    ///    إمّا تضيف لها seeding مماثلة أو تستبدل إنشاء المشتريات/المبيعات بمناداة الـservices الخاصة بك.
    /// </summary>
    public static class OpticsDemoSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await db.Database.MigrateAsync();

            // لو الديمو اتزرعت قبل كده، اخرج.
            if (await db.Set<CustomerClient>().AnyAsync(x => x.CustomerNumber == "C-0001"))
                return;

            var utcNow = DateTime.UtcNow;

            // -----------------------------------------------------------------
            // 1) Warehouses
            // -----------------------------------------------------------------
            var whMain = new Warehouse
            {
                Id = Guid.NewGuid(),
                WarehouseCode = "WH-OPT-01",
                WarehouseNameAr = "المخزن الرئيسي",
                WarehouseNameEn = "Main Store",
                IsActive = true,
                CreatedAtUtc = utcNow
            };
            var whLab = new Warehouse
            {
                Id = Guid.NewGuid(),
                WarehouseCode = "WH-LAB-01",
                WarehouseNameAr = "مخزن المعمل",
                WarehouseNameEn = "Lab Store",
                IsActive = true,
                CreatedAtUtc = utcNow
            };
            var whAcc = new Warehouse
            {
                Id = Guid.NewGuid(),
                WarehouseCode = "WH-ACC-01",
                WarehouseNameAr = "مخزن الإكسسوارات",
                WarehouseNameEn = "Accessories Store",
                IsActive = true,
                CreatedAtUtc = utcNow
            };
            await db.AddRangeAsync(whMain, whLab, whAcc);

            // -----------------------------------------------------------------
            // 2) Payment Methods
            // -----------------------------------------------------------------
            var pmCash = new PaymentMethod
            {
                Id = Guid.NewGuid(), MethodCode = "CASH", MethodNameAr = "نقدي", IsActive = true, CreatedAtUtc = utcNow
            };
            var pmVisa = new PaymentMethod
            {
                Id = Guid.NewGuid(), MethodCode = "VISA", MethodNameAr = "Visa / POS", IsActive = true, CreatedAtUtc = utcNow
            };
            var pmBank = new PaymentMethod
            {
                Id = Guid.NewGuid(), MethodCode = "BANK", MethodNameAr = "تحويل بنكي", IsActive = true, CreatedAtUtc = utcNow
            };
            var pmInst = new PaymentMethod
            {
                Id = Guid.NewGuid(), MethodCode = "INST", MethodNameAr = "آجل / على الحساب", IsActive = true, CreatedAtUtc = utcNow
            };
            var pmWallet = new PaymentMethod
            {
                Id = Guid.NewGuid(), MethodCode = "WALLET", MethodNameAr = "محفظة إلكترونية", IsActive = true, CreatedAtUtc = utcNow
            };
            await db.AddRangeAsync(pmCash, pmVisa, pmBank, pmInst, pmWallet);

            // -----------------------------------------------------------------
            // 3) Expense Categories
            // -----------------------------------------------------------------
            var expRent = new ExpenseCategory { Id = Guid.NewGuid(), CategoryCode = "EXP-RENT", NameAr = "إيجار", IsActive = true, CreatedAtUtc = utcNow };
            var expSal = new ExpenseCategory { Id = Guid.NewGuid(), CategoryCode = "EXP-SAL", NameAr = "مرتبات", IsActive = true, CreatedAtUtc = utcNow };
            var expElec = new ExpenseCategory { Id = Guid.NewGuid(), CategoryCode = "EXP-ELEC", NameAr = "كهرباء", IsActive = true, CreatedAtUtc = utcNow };
            var expMaint = new ExpenseCategory { Id = Guid.NewGuid(), CategoryCode = "EXP-MAINT", NameAr = "صيانة", IsActive = true, CreatedAtUtc = utcNow };
            var expTrans = new ExpenseCategory { Id = Guid.NewGuid(), CategoryCode = "EXP-TRANS", NameAr = "مواصلات", IsActive = true, CreatedAtUtc = utcNow };
            var expMisc = new ExpenseCategory { Id = Guid.NewGuid(), CategoryCode = "EXP-MISC", NameAr = "مصروفات عامة", IsActive = true, CreatedAtUtc = utcNow };
            await db.AddRangeAsync(expRent, expSal, expElec, expMaint, expTrans, expMisc);

            // -----------------------------------------------------------------
            // 4) Suppliers
            // -----------------------------------------------------------------
            var sup1 = NewSupplier("SUP-001", "شركة النور للعدسات", "محمد علي", "01010000001", "القاهرة", utcNow);
            var sup2 = NewSupplier("SUP-002", "مؤسسة الرؤية للإطارات", "أحمد سامي", "01010000002", "الجيزة", utcNow);
            var sup3 = NewSupplier("SUP-003", "هاي أوبتيك للمستلزمات", "وليد حسن", "01010000003", "الإسكندرية", utcNow);
            var sup4 = NewSupplier("SUP-004", "العدسة الحديثة", "كريم فتحي", "01010000004", "المنصورة", utcNow);
            await db.AddRangeAsync(sup1, sup2, sup3, sup4);

            // -----------------------------------------------------------------
            // 5) Items
            // -----------------------------------------------------------------
            var items = new List<Item>
            {
                NewFrame("FR-001", "إطار رجالي أسود Metal Classic", "RayOne", "M-100", "Black", 54, 18, 140, 950, utcNow),
                NewFrame("FR-002", "إطار حريمي ذهبي Elegant Lady", "Bella", "W-210", "Gold", 52, 17, 138, 1150, utcNow),
                NewFrame("FR-003", "إطار أطفال Blue Kids", "KidsPro", "K-050", "Blue", 46, 16, 130, 650, utcNow),
                NewFrame("FR-004", "إطار Titanium Premium", "Titan", "T-900", "Gray", 55, 18, 145, 2200, utcNow),
                NewFrame("FR-005", "إطار رياضي Flex Fit", "Active", "S-300", "Black", 57, 17, 145, 1450, utcNow),

                NewLens("LN-001", "عدسة سنجل Vision 1.56 Hard Coat", "CR39", "1.56", "Hard Coat", 250, utcNow),
                NewLens("LN-002", "عدسة بلو كت 1.56", "CR39", "1.56", "Blue Cut", 380, utcNow),
                NewLens("LN-003", "عدسة أنتي ريفلكشن 1.61", "MR8", "1.61", "AR", 520, utcNow),
                NewLens("LN-004", "عدسة Progressive Premium 1.67", "MR10", "1.67", "AR+UV", 1450, utcNow),
                NewLens("LN-005", "عدسة Photochromic 1.56", "CR39", "1.56", "Photo", 780, utcNow),

                NewContactLens("CL-001", "عدسات لاصقة شهرية Soft Monthly", "AquaLens", "M-30", null, 320, utcNow),
                NewContactLens("CL-002", "عدسات لاصقة يومية Daily Clear", "AquaLens", "D-01", null, 420, utcNow),
                NewContactLens("CL-003", "عدسات لاصقة ملونة Hazel Glow", "ColorEye", "C-88", "Hazel", 280, utcNow),

                NewAccessory("AC-001", "جراب نظارة فاخر", "Black", 120, utcNow),
                NewAccessory("AC-002", "فوطة تنظيف مايكروفايبر", "Blue", 35, utcNow),
                NewAccessory("AC-003", "محلول عدسات 250ml", null, 95, utcNow),
                NewAccessory("AC-004", "سلسلة نظارة", "Silver", 60, utcNow),

                NewService("SV-001", "تركيب عدسات", 80, utcNow),
                NewService("SV-002", "صيانة إطار", 50, utcNow),
                NewService("SV-003", "تنظيف وضبط نظارة", 30, utcNow),
            };
            await db.AddRangeAsync(items);

            // -----------------------------------------------------------------
            // 6) Customers
            // -----------------------------------------------------------------
            var c1 = NewCustomer("C-0001", "محمد أحمد عبدالعال", CustomerGender.Male, 34, "0931234567", "01090000001", "مراجع قديم", utcNow);
            var c2 = NewCustomer("C-0002", "منى خالد حسن", CustomerGender.Female, 29, "0931234568", "01090000002", "تعمل كمبيوتر", utcNow);
            var c3 = NewCustomer("C-0003", "يوسف علاء", CustomerGender.Male, 12, "0931234569", "01090000003", "طفل", utcNow);
            var c4 = NewCustomer("C-0004", "سارة محمود", CustomerGender.Female, 41, "0931234570", "01090000004", "تحتاج متابعة", utcNow);
            var c5 = NewCustomer("C-0005", "عبدالله رجب", CustomerGender.Male, 53, "0931234571", "01090000005", "عميل حساب", utcNow);
            var c6 = NewCustomer("C-0006", "ندى سمير", CustomerGender.Female, 24, "0931234572", "01090000006", "عدسات لاصقة", utcNow);
            var c7 = NewCustomer("C-0007", "كريم جمال", CustomerGender.Male, 37, "0931234573", "01090000007", "استلام متأخر", utcNow);
            var c8 = NewCustomer("C-0008", "مريم أسامة", CustomerGender.Female, 31, "0931234574", "01090000008", "عميلة جديدة", utcNow);
            await db.AddRangeAsync(c1, c2, c3, c4, c5, c6, c7, c8);

            await db.SaveChangesAsync();

            var item = items.ToDictionary(x => x.ItemCode, x => x);

            // -----------------------------------------------------------------
            // 7) Prescriptions
            // -----------------------------------------------------------------
            var p1 = NewPrescription(c1, new DateTime(2026, 3, 10), "Dr. Hossam", -1.50m, -0.50m, 180, -1.75m, -0.75m, 170, 0, 62, 18, "شاشة كمبيوتر", utcNow);
            var p2 = NewPrescription(c2, new DateTime(2026, 3, 18), "Dr. Nader", -2.25m, -1.00m, 175, -2.00m, -0.75m, 165, 0, 61, 19, "Anti-reflective preferred", utcNow);
            var p3 = NewPrescription(c3, new DateTime(2026, 3, 20), "Dr. Hany", -0.75m, 0, 0, -1.00m, 0, 0, 0, 58, 17, "طفل - استخدام يومي", utcNow);
            var p4 = NewPrescription(c4, new DateTime(2026, 3, 25), "Dr. Samir", 1.50m, -0.50m, 90, 1.25m, -0.25m, 100, 1.50m, 63, 20, "Progressive", utcNow);
            var p5 = NewPrescription(c5, new DateTime(2026, 3, 28), "Dr. Magdy", -3.00m, -1.25m, 180, -2.75m, -1.00m, 170, 0, 64, 19, "عميل حساب", utcNow);
            var p6 = NewPrescription(c6, new DateTime(2026, 4, 1), "Dr. Salma", -1.00m, 0, 0, -1.25m, 0, 0, 0, 60, 18, "Contact lens", utcNow);
            var p7 = NewPrescription(c7, new DateTime(2026, 4, 3), "Dr. Adel", -2.50m, -0.50m, 180, -2.75m, -0.50m, 170, 0, 63, 18, "مستعجل", utcNow);
            var p8 = NewPrescription(c8, new DateTime(2026, 4, 5), "Dr. Yasser", -1.25m, -0.25m, 175, -1.50m, -0.50m, 165, 0, 61, 18, "أول زيارة", utcNow);
            await db.AddRangeAsync(p1, p2, p3, p4, p5, p6, p7, p8);

            // -----------------------------------------------------------------
            // 8) Old Records
            // -----------------------------------------------------------------
            await db.AddRangeAsync(
                NewOldRecord(c1, new DateTime(2025, 11, 15), "نظارة قديمة", "كان يستخدم إطار معدني وعدسة 1.56", utcNow),
                NewOldRecord(c2, new DateTime(2025, 12, 20), "شكوى انعكاس", "اشتكت من الوهج الليلي", utcNow),
                NewOldRecord(c4, new DateTime(2025, 10, 5), "متابعة تقدم السن", "أوصي بعدسة Progressive", utcNow),
                NewOldRecord(c5, new DateTime(2025, 9, 30), "عميل حساب قديم", "تم الاتفاق على سداد شهري", utcNow),
                NewOldRecord(c7, new DateTime(2025, 8, 18), "كسر إطار سابق", "تم تغيير الإطار فقط", utcNow)
            );

            await db.SaveChangesAsync();

            // -----------------------------------------------------------------
            // 9) Purchase Invoices
            // -----------------------------------------------------------------
            var pur1 = NewPurchaseInvoice("PUR-0001", new DateTime(2026, 3, 1), sup2, whMain, utcNow,
                NewPurchaseLine(item["FR-001"], 10, 550),
                NewPurchaseLine(item["FR-002"], 8, 700),
                NewPurchaseLine(item["FR-003"], 12, 350));

            var pur2 = NewPurchaseInvoice("PUR-0002", new DateTime(2026, 3, 2), sup1, whLab, utcNow,
                NewPurchaseLine(item["LN-001"], 40, 120),
                NewPurchaseLine(item["LN-002"], 30, 180),
                NewPurchaseLine(item["LN-003"], 25, 260));

            var pur3 = NewPurchaseInvoice("PUR-0003", new DateTime(2026, 3, 3), sup4, whLab, utcNow,
                NewPurchaseLine(item["LN-004"], 10, 900),
                NewPurchaseLine(item["LN-005"], 15, 420));

            var pur4 = NewPurchaseInvoice("PUR-0004", new DateTime(2026, 3, 4), sup3, whAcc, utcNow,
                NewPurchaseLine(item["AC-001"], 30, 60),
                NewPurchaseLine(item["AC-002"], 100, 12),
                NewPurchaseLine(item["AC-003"], 50, 45),
                NewPurchaseLine(item["AC-004"], 40, 22));

            var pur5 = NewPurchaseInvoice("PUR-0005", new DateTime(2026, 3, 5), sup3, whMain, utcNow,
                NewPurchaseLine(item["CL-001"], 20, 180),
                NewPurchaseLine(item["CL-002"], 15, 250),
                NewPurchaseLine(item["CL-003"], 25, 140));

            var pur6 = NewPurchaseInvoice("PUR-0006", new DateTime(2026, 3, 6), sup1, whLab, utcNow,
                NewPurchaseLine(item["SV-001"], 100, 0),
                NewPurchaseLine(item["SV-002"], 100, 0),
                NewPurchaseLine(item["SV-003"], 100, 0));

            await db.AddRangeAsync(pur1, pur2, pur3, pur4, pur5, pur6);
            await db.SaveChangesAsync();

            // -----------------------------------------------------------------
            // 10) Sales Invoices
            // -----------------------------------------------------------------
            var inv1 = NewSalesInvoice("INV-OPT-0001", new DateTime(2026, 3, 11), c1, p1, whMain, utcNow,
                NewSalesLine(item["FR-001"], 1, 950),
                NewSalesLine(item["LN-002"], 2, 380),
                NewSalesLine(item["SV-001"], 1, 80),
                NewSalesLine(item["AC-001"], 1, 120));
            AddPayments(inv1, pmCash, 500, pmInst, 1410);

            var inv2 = NewSalesInvoice("INV-OPT-0002", new DateTime(2026, 3, 19), c2, p2, whMain, utcNow,
                NewSalesLine(item["FR-002"], 1, 1150),
                NewSalesLine(item["LN-003"], 2, 520),
                NewSalesLine(item["SV-001"], 1, 80));
            AddPayments(inv2, pmVisa, 2270);

            var inv3 = NewSalesInvoice("INV-OPT-0003", new DateTime(2026, 3, 21), c3, p3, whMain, utcNow,
                NewSalesLine(item["FR-003"], 1, 650),
                NewSalesLine(item["LN-001"], 2, 250),
                NewSalesLine(item["SV-001"], 1, 80));
            AddPayments(inv3, pmCash, 1230);

            var inv4 = NewSalesInvoice("INV-OPT-0004", new DateTime(2026, 3, 26), c4, p4, whMain, utcNow,
                NewSalesLine(item["FR-004"], 1, 2200),
                NewSalesLine(item["LN-004"], 2, 1450),
                NewSalesLine(item["SV-001"], 1, 80));
            AddPayments(inv4, pmInst, 5180);

            var inv5 = NewSalesInvoice("INV-OPT-0005", new DateTime(2026, 3, 29), c5, p5, whMain, utcNow,
                NewSalesLine(item["FR-005"], 1, 1450),
                NewSalesLine(item["LN-003"], 2, 520),
                NewSalesLine(item["AC-001"], 1, 120));
            AddPayments(inv5, pmBank, 1000, pmInst, 1610);

            var inv6 = NewSalesInvoice("INV-OPT-0006", new DateTime(2026, 4, 1), c6, p6, whMain, utcNow,
                NewSalesLine(item["CL-001"], 1, 320),
                NewSalesLine(item["AC-003"], 1, 95));
            AddPayments(inv6, pmCash, 415);

            var inv7 = NewSalesInvoice("INV-OPT-0007", new DateTime(2026, 4, 3), c7, p7, whMain, utcNow,
                NewSalesLine(item["FR-001"], 1, 950),
                NewSalesLine(item["LN-005"], 2, 780),
                NewSalesLine(item["SV-001"], 1, 80));
            AddPayments(inv7, pmCash, 500, pmInst, 2090);

            var inv8 = NewSalesInvoice("INV-OPT-0008", new DateTime(2026, 4, 5), c8, p8, whMain, utcNow,
                NewSalesLine(item["FR-002"], 1, 1150),
                NewSalesLine(item["LN-002"], 2, 380),
                NewSalesLine(item["SV-001"], 1, 80),
                NewSalesLine(item["AC-002"], 1, 35));
            AddPayments(inv8, pmVisa, 2025);

            var invCash = NewSalesInvoice("INV-CASH-0001", new DateTime(2026, 4, 6), null, null, whAcc, utcNow,
                NewSalesLine(item["AC-001"], 1, 120),
                NewSalesLine(item["AC-002"], 2, 35));
            invCash.CustomerName = "Cash Sale";
            AddPayments(invCash, pmCash, 190);

            await db.AddRangeAsync(inv1, inv2, inv3, inv4, inv5, inv6, inv7, inv8, invCash);
            await db.SaveChangesAsync();

            // -----------------------------------------------------------------
            // 11) Sales Return
            // -----------------------------------------------------------------
            var sr1 = new SalesReturnInvoice
            {
                Id = Guid.NewGuid(),
                ReturnNumber = "SR-0001",
                ReturnDateUtc = new DateTime(2026, 4, 7),
                OriginalSalesInvoiceId = inv6.Id,
                OriginalSalesInvoice = inv6,
                CustomerName = c6.NameAr,
                WarehouseId = whMain.Id,
                Warehouse = whMain,
                Status = SalesReturnStatus.Posted,
                CreatedAtUtc = utcNow
            };
            var srLine = inv6.Lines.First(x => x.ItemId == item["AC-003"].Id);
            sr1.Lines.Add(new SalesReturnLine
            {
                Id = Guid.NewGuid(),
                SalesReturnInvoiceId = sr1.Id,
                OriginalSalesInvoiceLineId = srLine.Id,
                OriginalSalesInvoiceLine = srLine,
                ItemId = srLine.ItemId,
                Item = srLine.Item,
                Quantity = 1,
                UnitPrice = 95,
                DiscountAmount = 0,
                TaxAmount = 0,
                LineTotal = 95
            });
            sr1.SubTotal = 95;
            sr1.DiscountAmount = 0;
            sr1.TaxAmount = 0;
            sr1.NetAmount = 95;
            await db.AddAsync(sr1);

            // -----------------------------------------------------------------
            // 12) Customer Receipts
            // -----------------------------------------------------------------
            var rec1 = NewCustomerReceipt("REC-0001", new DateTime(2026, 3, 15), c1, pmCash, 500, "عربون إضافي", utcNow);
            var rec2 = NewCustomerReceipt("REC-0002", new DateTime(2026, 3, 30), c5, pmBank, 1000, "سداد جزء من الحساب", utcNow);
            var rec3 = NewCustomerReceipt("REC-0003", new DateTime(2026, 4, 8), c7, pmCash, 1000, "دفعة قبل الاستلام", utcNow);
            var rec4 = NewCustomerReceipt("REC-0004", new DateTime(2026, 4, 9), c4, pmVisa, 2000, "دفعة من العميل", utcNow);
            await db.AddRangeAsync(rec1, rec2, rec3, rec4);

            // -----------------------------------------------------------------
            // 13) Customer Account Transactions (علشان تبويب Account يبان فورًا)
            // -----------------------------------------------------------------
            var accountTx = new List<CustomerAccountTransaction>();
            accountTx.AddRange(BuildSaleAccountTx(inv1));
            accountTx.AddRange(BuildSaleAccountTx(inv2));
            accountTx.AddRange(BuildSaleAccountTx(inv3));
            accountTx.AddRange(BuildSaleAccountTx(inv4));
            accountTx.AddRange(BuildSaleAccountTx(inv5));
            accountTx.AddRange(BuildSaleAccountTx(inv6));
            accountTx.AddRange(BuildSaleAccountTx(inv7));
            accountTx.AddRange(BuildSaleAccountTx(inv8));
            accountTx.Add(BuildSalesReturnAccountTx(sr1, c6.Id));
            accountTx.Add(BuildReceiptAccountTx(rec1));
            accountTx.Add(BuildReceiptAccountTx(rec2));
            accountTx.Add(BuildReceiptAccountTx(rec3));
            accountTx.Add(BuildReceiptAccountTx(rec4));
            await db.AddRangeAsync(accountTx);

            // -----------------------------------------------------------------
            // 14) Expenses
            // -----------------------------------------------------------------
            await db.AddRangeAsync(
                NewExpense("EXP-0001", new DateTime(2026, 4, 1), expRent, pmBank, 12000, "إيجار المحل لشهر أبريل", utcNow),
                NewExpense("EXP-0002", new DateTime(2026, 4, 2), expSal, pmCash, 8000, "جزء من رواتب العاملين", utcNow),
                NewExpense("EXP-0003", new DateTime(2026, 4, 3), expElec, pmCash, 1350, "فاتورة كهرباء", utcNow),
                NewExpense("EXP-0004", new DateTime(2026, 4, 4), expMaint, pmCash, 600, "صيانة ماكينة", utcNow),
                NewExpense("EXP-0005", new DateTime(2026, 4, 5), expMisc, pmWallet, 250, "ضيافة ومستلزمات", utcNow),
                NewExpense("EXP-0006", new DateTime(2026, 4, 6), expTrans, pmCash, 180, "مواصلات المعمل", utcNow)
            );

            // -----------------------------------------------------------------
            // 15) Optical Work Orders
            // -----------------------------------------------------------------
            await db.AddRangeAsync(
                NewWorkOrder("WO-0001", inv1, c1, p1, new DateTime(2026, 3, 11), new DateTime(2026, 3, 14), OpticalWorkOrderStatus.Delivered, false,
                    "ضبط الإطار بعد التركيب", "Blue cut", "تم التركيب والضبط", "تم التسليم للعميل", utcNow,
                    readyDate: new DateTime(2026, 3, 14), deliveredDate: new DateTime(2026, 3, 14)),
                NewWorkOrder("WO-0002", inv2, c2, p2, new DateTime(2026, 3, 19), new DateTime(2026, 3, 22), OpticalWorkOrderStatus.Ready, false,
                    "تأكيد اللون الذهبي", "AR coating", "جاهز على الرف", "في انتظار استلام العميل", utcNow,
                    readyDate: new DateTime(2026, 3, 22)),
                NewWorkOrder("WO-0003", inv3, c3, p3, new DateTime(2026, 3, 21), new DateTime(2026, 3, 23), OpticalWorkOrderStatus.Delivered, false,
                    null, null, null, null, utcNow,
                    deliveredDate: new DateTime(2026, 3, 23)),
                NewWorkOrder("WO-0004", inv4, c4, p4, new DateTime(2026, 3, 26), new DateTime(2026, 3, 30), OpticalWorkOrderStatus.InLab, false,
                    "Progressive frame alignment", "Progressive premium", "تحت التجهيز", null, utcNow),
                NewWorkOrder("WO-0005", inv5, c5, p5, new DateTime(2026, 3, 29), new DateTime(2026, 4, 2), OpticalWorkOrderStatus.New, false,
                    null, null, null, null, utcNow),
                NewWorkOrder("WO-0006", inv7, c7, p7, new DateTime(2026, 4, 3), new DateTime(2026, 4, 5), OpticalWorkOrderStatus.Ready, true,
                    "مستعجل جدًا", "Photochromic", "جاهز من الأمس", "العميل لم يستلم بعد", utcNow,
                    readyDate: new DateTime(2026, 4, 5)),
                NewWorkOrder("WO-0007", inv8, c8, p8, new DateTime(2026, 4, 5), new DateTime(2026, 4, 10), OpticalWorkOrderStatus.InLab, false,
                    null, null, null, null, utcNow)
            );

            await db.SaveChangesAsync();
        }

        // -------------------------------------------------------------
        // Helper constructors
        // -------------------------------------------------------------
        private static Supplier NewSupplier(string no, string name, string contact, string mobile, string address, DateTime utcNow)
            => new Supplier
            {
                Id = Guid.NewGuid(),
                SupplierNumber = no,
                NameAr = name,
                ContactPerson = contact,
                MobileNo = mobile,
                Address = address,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

        private static Item NewFrame(string code, string name, string brand, string model, string color, decimal eye, decimal bridge, decimal temple, decimal salePrice, DateTime utcNow)
            => new Item
            {
                Id = Guid.NewGuid(),
                ItemCode = code,
                ItemNameAr = name,
                OpticalItemType = OpticalItemType.Frame,
                BrandName = brand,
                ModelName = model,
                Color = color,
                EyeSize = eye,
                BridgeSize = bridge,
                TempleLength = temple,
                RequiresPrescription = false,
                SalePrice = salePrice,
                IsStockItem = true,
                IsService = false,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

        private static Item NewLens(string code, string name, string material, string index, string coating, decimal salePrice, DateTime utcNow)
            => new Item
            {
                Id = Guid.NewGuid(),
                ItemCode = code,
                ItemNameAr = name,
                OpticalItemType = OpticalItemType.Lens,
                LensMaterial = material,
                LensIndex = index,
                LensCoating = coating,
                RequiresPrescription = true,
                SalePrice = salePrice,
                IsStockItem = true,
                IsService = false,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

        private static Item NewContactLens(string code, string name, string brand, string model, string? color, decimal salePrice, DateTime utcNow)
            => new Item
            {
                Id = Guid.NewGuid(),
                ItemCode = code,
                ItemNameAr = name,
                OpticalItemType = OpticalItemType.ContactLens,
                BrandName = brand,
                ModelName = model,
                Color = color,
                RequiresPrescription = false,
                SalePrice = salePrice,
                IsStockItem = true,
                IsService = false,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

        private static Item NewAccessory(string code, string name, string? color, decimal salePrice, DateTime utcNow)
            => new Item
            {
                Id = Guid.NewGuid(),
                ItemCode = code,
                ItemNameAr = name,
                OpticalItemType = OpticalItemType.Accessory,
                Color = color,
                RequiresPrescription = false,
                SalePrice = salePrice,
                IsStockItem = true,
                IsService = false,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

        private static Item NewService(string code, string name, decimal salePrice, DateTime utcNow)
            => new Item
            {
                Id = Guid.NewGuid(),
                ItemCode = code,
                ItemNameAr = name,
                OpticalItemType = OpticalItemType.Service,
                RequiresPrescription = false,
                SalePrice = salePrice,
                IsStockItem = false,
                IsService = true,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

        private static CustomerClient NewCustomer(string no, string name, CustomerGender gender, int age, string tel, string mobile, string remarks, DateTime utcNow)
            => new CustomerClient
            {
                Id = Guid.NewGuid(),
                CustomerNumber = no,
                NameAr = name,
                Gender = gender,
                Age = age,
                Tel = tel,
                MobileNo = mobile,
                Remarks = remarks,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

        private static Prescription NewPrescription(CustomerClient customer, DateTime date, string doctor,
            decimal? rs, decimal? rc, decimal? ra,
            decimal? ls, decimal? lc, decimal? la,
            decimal? add, decimal? ipd, decimal? sh, string remarks, DateTime utcNow)
            => new Prescription
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Customer = customer,
                PrescriptionDateUtc = date,
                DoctorName = doctor,
                RightSph = rs,
                RightCyl = rc,
                RightAxis = ra,
                LeftSph = ls,
                LeftCyl = lc,
                LeftAxis = la,
                AddValue = add,
                IPD = ipd,
                SHeight = sh,
                Remarks = remarks,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

        private static CustomerOldRecord NewOldRecord(CustomerClient customer, DateTime date, string title, string details, DateTime utcNow)
            => new CustomerOldRecord
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Customer = customer,
                RecordDateUtc = date,
                Title = title,
                Details = details,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

        private static PurchaseInvoice NewPurchaseInvoice(string no, DateTime date, Supplier supplier, Warehouse warehouse, DateTime utcNow, params PurchaseInvoiceLine[] lines)
        {
            var inv = new PurchaseInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = no,
                InvoiceDateUtc = date,
                SupplierId = supplier.Id,
                Supplier = supplier,
                SupplierName = supplier.NameAr,
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                Status = PurchaseInvoiceStatus.Posted,
                CreatedAtUtc = utcNow
            };

            foreach (var line in lines)
            {
                line.Id = Guid.NewGuid();
                line.PurchaseInvoiceId = inv.Id;
                line.PurchaseInvoice = inv;
                inv.Lines.Add(line);
            }

            inv.SubTotal = inv.Lines.Sum(x => x.LineTotal);
            inv.DiscountAmount = 0;
            inv.TaxAmount = 0;
            inv.NetAmount = inv.SubTotal;
            return inv;
        }

        private static PurchaseInvoiceLine NewPurchaseLine(Item item, decimal qty, decimal unitCost)
            => new PurchaseInvoiceLine
            {
                ItemId = item.Id,
                Item = item,
                Quantity = qty,
                UnitCost = unitCost,
                DiscountAmount = 0,
                TaxAmount = 0,
                LineTotal = qty * unitCost
            };

        private static SalesInvoice NewSalesInvoice(string no, DateTime date, CustomerClient? customer, Prescription? prescription, Warehouse warehouse, DateTime utcNow, params SalesInvoiceLine[] lines)
        {
            var inv = new SalesInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = no,
                InvoiceDateUtc = date,
                CustomerId = customer?.Id,
                Customer = customer,
                PrescriptionId = prescription?.Id,
                Prescription = prescription,
                CustomerName = customer?.NameAr ?? "Cash Sale",
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                Status = SalesInvoiceStatus.Posted,
                CreatedAtUtc = utcNow
            };

            foreach (var line in lines)
            {
                line.Id = Guid.NewGuid();
                line.SalesInvoiceId = inv.Id;
                line.SalesInvoice = inv;
                inv.Lines.Add(line);
            }

            inv.SubTotal = inv.Lines.Sum(x => x.LineTotal);
            inv.DiscountAmount = 0;
            inv.TaxAmount = 0;
            inv.NetAmount = inv.SubTotal;
            return inv;
        }

        private static SalesInvoiceLine NewSalesLine(Item item, decimal qty, decimal unitPrice)
            => new SalesInvoiceLine
            {
                ItemId = item.Id,
                Item = item,
                Quantity = qty,
                UnitPrice = unitPrice,
                DiscountAmount = 0,
                TaxAmount = 0,
                LineTotal = qty * unitPrice
            };

        private static void AddPayments(SalesInvoice invoice, PaymentMethod method1, decimal amount1, PaymentMethod? method2 = null, decimal amount2 = 0)
        {
            invoice.Payments.Add(new SalesInvoicePayment
            {
                Id = Guid.NewGuid(),
                SalesInvoiceId = invoice.Id,
                SalesInvoice = invoice,
                PaymentMethodId = method1.Id,
                PaymentMethod = method1,
                Amount = amount1
            });

            if (method2 != null && amount2 > 0)
            {
                invoice.Payments.Add(new SalesInvoicePayment
                {
                    Id = Guid.NewGuid(),
                    SalesInvoiceId = invoice.Id,
                    SalesInvoice = invoice,
                    PaymentMethodId = method2.Id,
                    PaymentMethod = method2,
                    Amount = amount2
                });
            }
        }

        private static CustomerReceipt NewCustomerReceipt(string no, DateTime date, CustomerClient customer, PaymentMethod paymentMethod, decimal amount, string notes, DateTime utcNow)
            => new CustomerReceipt
            {
                Id = Guid.NewGuid(),
                ReceiptNumber = no,
                ReceiptDateUtc = date,
                CustomerId = customer.Id,
                Customer = customer,
                PaymentMethodId = paymentMethod.Id,
                PaymentMethod = paymentMethod,
                Amount = amount,
                Notes = notes,
                Status = CustomerReceiptStatus.Posted,
                CreatedAtUtc = utcNow
            };

        private static Expensex NewExpense(string no, DateTime date, ExpenseCategory category, PaymentMethod method, decimal amount, string description, DateTime utcNow)
            => new Expensex
            {
                Id = Guid.NewGuid(),
                ExpenseNumber = no,
                ExpenseDateUtc = date,
                ExpenseCategoryId = category.Id,
                ExpenseCategory = category,
                PaymentMethodId = method.Id,
                PaymentMethod = method,
                Amount = amount,
                Description = description,
                IsPosted = true,
                CreatedAtUtc = utcNow
            };

        private static OpticalWorkOrder NewWorkOrder(string no, SalesInvoice invoice, CustomerClient customer, Prescription? prescription,
            DateTime orderDate, DateTime? expectedDelivery, OpticalWorkOrderStatus status, bool urgent,
            string? frameNotes, string? lensNotes, string? workshopNotes, string? deliveryNotes, DateTime utcNow,
            DateTime? readyDate = null, DateTime? deliveredDate = null)
            => new OpticalWorkOrder
            {
                Id = Guid.NewGuid(),
                WorkOrderNumber = no,
                SalesInvoiceId = invoice.Id,
                SalesInvoice = invoice,
                CustomerId = customer.Id,
                Customer = customer,
                PrescriptionId = prescription?.Id,
                Prescription = prescription,
                OrderDateUtc = orderDate,
                ExpectedDeliveryDateUtc = expectedDelivery,
                ReadyDateUtc = readyDate,
                DeliveredDateUtc = deliveredDate,
                Status = status,
                IsUrgent = urgent,
                FrameNotes = frameNotes,
                LensNotes = lensNotes,
                WorkshopNotes = workshopNotes,
                DeliveryNotes = deliveryNotes,
                CreatedAtUtc = utcNow
            };

        private static IEnumerable<CustomerAccountTransaction> BuildSaleAccountTx(SalesInvoice invoice)
        {
            if (!invoice.CustomerId.HasValue)
                return Enumerable.Empty<CustomerAccountTransaction>();

            return new[]
            {
                new CustomerAccountTransaction
                {
                    Id = Guid.NewGuid(),
                    CustomerId = invoice.CustomerId.Value,
                    TransactionDateUtc = invoice.InvoiceDateUtc,
                    TransactionType = CustomerAccountTransactionType.SaleInvoice,
                    ReferenceType = "SalesInvoice",
                    ReferenceId = invoice.Id,
                    ReferenceNumber = invoice.InvoiceNumber,
                    Description = $"فاتورة بيع رقم {invoice.InvoiceNumber}",
                    DebitAmount = invoice.NetAmount,
                    CreditAmount = 0,
                    CreatedAtUtc = DateTime.UtcNow
                }
            };
        }

        private static CustomerAccountTransaction BuildSalesReturnAccountTx(SalesReturnInvoice ret, Guid customerId)
            => new CustomerAccountTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                TransactionDateUtc = ret.ReturnDateUtc,
                TransactionType = CustomerAccountTransactionType.SalesReturn,
                ReferenceType = "SalesReturn",
                ReferenceId = ret.Id,
                ReferenceNumber = ret.ReturnNumber,
                Description = $"مرتجع مبيعات رقم {ret.ReturnNumber}",
                DebitAmount = 0,
                CreditAmount = ret.NetAmount,
                CreatedAtUtc = DateTime.UtcNow
            };

        private static CustomerAccountTransaction BuildReceiptAccountTx(CustomerReceipt receipt)
            => new CustomerAccountTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = receipt.CustomerId,
                TransactionDateUtc = receipt.ReceiptDateUtc,
                TransactionType = CustomerAccountTransactionType.Receipt,
                ReferenceType = "CustomerReceipt",
                ReferenceId = receipt.Id,
                ReferenceNumber = receipt.ReceiptNumber,
                Description = $"سند قبض رقم {receipt.ReceiptNumber}",
                DebitAmount = 0,
                CreditAmount = receipt.Amount,
                CreatedAtUtc = DateTime.UtcNow
            };
    }
}
