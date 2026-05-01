using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF2
{
    /// <summary>
    /// Large Scale Seeder — يكمّل إلى 10,000 مستفيد وبيانات متناسبة في كل الموديولات
    /// Idempotent: يشغّل فقط الفرق من الرقم الحالي للهدف
    /// </summary>
    public static class LargeScaleSeeder
    {
        private const int TargetBeneficiaries = 10_000;

        private static readonly Random Rng = new(42);

        private static readonly string[] MaleFirst = { "أحمد", "محمد", "محمود", "خالد", "عبدالله", "مصطفى", "هيثم", "حسام", "طارق", "وائل", "علي", "حسن", "ياسر", "إسلام", "عمر", "بلال", "ماجد", "شريف", "رامي", "سامي" };
        private static readonly string[] FemaleFirst = { "فاطمة", "أسماء", "مريم", "آية", "رحاب", "هدى", "سارة", "نهى", "إيمان", "دعاء", "ولاء", "شيماء", "ندى", "رانيا", "هالة", "نورا", "سمر", "منى", "ريم", "لمياء" };
        private static readonly string[] FamilyNames = { "عبدالسلام", "علي", "حسن", "عبدالحميد", "السيد", "مصطفى", "طه", "صالح", "مبارك", "النوبي", "زيدان", "قنديل", "عوض", "سليمان", "حنفي", "عرفات", "جمعة", "فرج", "رجب", "ذكي" };
        private static readonly string[] Governorates = { "القاهرة", "الجيزة", "الإسكندرية", "أسيوط", "سوهاج", "المنيا", "الفيوم", "بني سويف", "قنا", "الأقصر", "أسوان", "الشرقية", "الدقهلية", "الغربية", "المنوفية", "كفر الشيخ", "دمياط", "الإسماعيلية", "بورسعيد", "السويس" };
        private static readonly string[] Cities = { "مركز السلام", "مركز الزيتون", "مركز طما", "مركز ساقلته", "مركز أبوتيج", "مركز الغنايم", "مركز ديروط", "مركز منفلوط", "مركز القوصية", "مركز البدري", "مدينة الخانكة", "مدينة شبين الكوم", "مدينة المحلة الكبرى", "مدينة طنطا", "مدينة دمياط", "مدينة رشيد", "مدينة إدفو", "مدينة الأقصر" };
        private static readonly string[] DonorNames = { "سعد محمد الأنصاري", "خالد إبراهيم التميمي", "نواف عبدالرحمن العتيبي", "محمد فهد الزهراني", "عبدالعزيز سلطان الشمري", "بدر ناصر القحطاني", "فيصل عمر الدوسري", "سلطان جابر المطيري", "حمدان راشد العنزي", "منصور طلال الغامدي", "مؤسسة الخير للأعمال الخيرية", "شركة التضامن للتنمية", "صندوق دعم الأسرة", "جمعية أصدقاء المجتمع", "مؤسسة بناء الأجيال" };

        private static string BuildName(bool male)
        {
            var first = male ? MaleFirst[Rng.Next(MaleFirst.Length)] : FemaleFirst[Rng.Next(FemaleFirst.Length)];
            var mid = MaleFirst[Rng.Next(MaleFirst.Length)];
            return $"{first} {mid} {FamilyNames[Rng.Next(FamilyNames.Length)]}";
        }

        // ══════════════════════════════════════════════════
        //  Entry Point
        // ══════════════════════════════════════════════════
        public static async Task SeedAsync(AppDbContext db, string? userId = null)
        {
            var existing = await db.Set<Beneficiary>()
                .CountAsync(x => x.Code.StartsWith("BEN-DEMO-"));

            if (existing >= TargetBeneficiaries) return;

            Console.WriteLine($"[LargeScaleSeeder] الحالي: {existing} — سيُضاف: {TargetBeneficiaries - existing}");

            var statusIds = await LoadStatusIdsAsync(db);
            var aidTypes = await db.Set<AidTypeLookup>().Where(x => x.IsActive).ToListAsync();
            var genders = await db.Set<GenderLookup>().OrderBy(x => x.DisplayOrder).Take(2).ToListAsync();
            var marital = await db.Set<MaritalStatusLookup>().ToListAsync();

            // ── 1. تكملة المستفيدين ──
            await SeedBeneficiariesAsync(db, existing, TargetBeneficiaries, statusIds, aidTypes, genders, marital, userId);

            // ── 2. تكملة الموظفين ──
            await SeedEmployeesAsync(db, userId);

            // ── 3. تكملة الكفلاء والكفالات ──
            await SeedKafalaAsync(db, userId);

            // ── 3. تكملة المتبرعين والتبرعات ──
            await SeedDonorsAndDonationsAsync(db, userId);

            // ── 4. اجتماعات إضافية ──
            await SeedBoardMeetingsAsync(db, userId);

            // ── 5. مقترحات مشاريع إضافية ──
            await SeedProjectProposalsAsync(db, userId);

            // ── 6. متطوعون إضافيون ──
            await SeedVolunteersAsync(db, userId);

            Console.WriteLine($"[LargeScaleSeeder] اكتمل بنجاح ✓");
        }

        // ══════════════════════════════════════════════════
        //  1. المستفيدون — تكملة حتى 10,000
        // ══════════════════════════════════════════════════
        private static async Task SeedBeneficiariesAsync(
            AppDbContext db, int from, int to,
            StatusIds st, List<AidTypeLookup> aidTypes,
            List<GenderLookup> genders, List<MaritalStatusLookup> marital,
            string? userId)
        {
            var batchSize = 500;
            var bens = new List<Beneficiary>(batchSize);
            var requests = new List<BeneficiaryAidRequest>(batchSize * 2);
            var decisions = new List<BeneficiaryCommitteeDecision>(batchSize);

            for (var i = from + 1; i <= to; i++)
            {
                var male = i % 3 != 0;
                var gov = Governorates[(i - 1) % Governorates.Length];
                var city = Cities[(i - 1) % Cities.Length];
                var status = i <= (to * 0.70) ? st.Approved
                           : i <= (to * 0.85) ? st.Review
                           : i <= (to * 0.93) ? st.New
                           : st.Stopped;

                var ben = new Beneficiary
                {
                    Id = Guid.NewGuid(),
                    Code = $"BEN-DEMO-{i:00000}",
                    FullName = BuildName(male),
                    NationalId = $"299{i:011}",
                    BirthDate = DateTime.Today.AddYears(-(22 + (i % 38))).AddDays(-(i % 300)),
                    GenderId = male ? genders[0].Id : (genders.Count > 1 ? genders[1].Id : genders[0].Id),
                    MaritalStatusId = marital.Count > 0 ? marital[i % marital.Count].Id : (Guid?)null,
                    PhoneNumber = $"0105{(1_000_000 + i % 8_999_999):0000000}",
                    AddressLine = $"{gov} - {city}",
                    FamilyMembersCount = 2 + (i % 7),
                    MonthlyIncome = 800m + (i % 50) * 120m,
                    IncomeSource = (i % 5) switch { 0 => "يومية غير ثابتة", 1 => "عمل بسيط", 2 => "معاش محدود", 3 => "مساعدة أقارب", _ => "عمل موسمي" },
                    HealthStatus = i % 7 == 0 ? "يحتاج متابعة صحية" : "مستقر",
                    WorkStatus = i % 5 == 0 ? "بدون عمل" : "عامل يومية",
                    HousingStatus = i % 4 == 0 ? "إيجار" : "ملك أسرة",
                    StatusId = status,
                    RegistrationDate = DateTime.Today.AddDays(-(10 + (i % 1000))),
                    CreatedByUserId = userId,
                    IsActive = i % 31 != 0
                };
                bens.Add(ben);

                // طلب مساعدة لـ 85% من المستفيدين
                if (i % 20 != 0 && aidTypes.Any())
                {
                    var aidType = aidTypes[i % aidTypes.Count];
                    requests.Add(new BeneficiaryAidRequest
                    {
                        Id = Guid.NewGuid(),
                        BeneficiaryId = ben.Id,
                        AidTypeId = aidType.Id,
                        RequestDate = DateTime.Today.AddDays(-(5 + (i % 400))),
                        RequestedAmount = 500m + (i % 20) * 100m,
                        Status = i % 4 == 0 ? "Approved" : i % 8 == 0 ? "Rejected" : "UnderReview",
                       
                        CreatedByUserId = userId
                    });
                }

                // قرار لجنة لـ 60% من المعتمدين
                if (status == st.Approved && i % 5 != 0)
                {
                    decisions.Add(new BeneficiaryCommitteeDecision
                    {
                        Id = Guid.NewGuid(),
                        BeneficiaryId = ben.Id,
                        DecisionDate = DateTime.Today.AddDays(-(30 + (i % 300))),
                        DecisionType = "موافقة",
                        ApprovedAmount = 600m + (i % 15) * 80m,
                        DurationInMonths = 3 + (i % 9),
                        ApprovedStatus = true
                    });
                }

                // حفظ كل 500 دفعة
                if (bens.Count >= batchSize)
                {
                    await db.Set<Beneficiary>().AddRangeAsync(bens);
                    await db.Set<BeneficiaryAidRequest>().AddRangeAsync(requests);
                    await db.Set<BeneficiaryCommitteeDecision>().AddRangeAsync(decisions);
                    await db.SaveChangesAsync();
                    Console.WriteLine($"[Beneficiaries] حُفظ حتى {i:00000}");
                    bens.Clear(); requests.Clear(); decisions.Clear();
                }
            }

            // الدفعة الأخيرة
            if (bens.Any())
            {
                await db.Set<Beneficiary>().AddRangeAsync(bens);
                await db.Set<BeneficiaryAidRequest>().AddRangeAsync(requests);
                await db.Set<BeneficiaryCommitteeDecision>().AddRangeAsync(decisions);
                await db.SaveChangesAsync();
            }
        }

        // ══════════════════════════════════════════════════
        //  2. الكفالات — تكملة حتى 1,500
        // ══════════════════════════════════════════════════
        private static async Task SeedKafalaAsync(AppDbContext db, string? userId)
        {
            const int target = 1_500;
            var existing = await db.Set<KafalaCase>()
                .CountAsync(x => x.CaseNumber.StartsWith("KAF-DEMO-"));
            if (existing >= target) return;

            var beneficiaryIds = await db.Set<Beneficiary>()
                .Where(x => x.Code.StartsWith("BEN-DEMO-") && x.IsActive)
                .OrderByDescending(x => x.Code)
                .Take(target * 2)
                .Select(x => x.Id)
                .ToListAsync();

            var sponsorStart = await db.Set<KafalaSponsor>()
                .Where(x => x.SponsorCode.StartsWith("KSP-DEMO-"))
                .CountAsync();

            var pmId = await db.Set<PaymentMethod>()
                .Where(x => x.IsActive && x.IsCash)
                .Select(x => x.Id).FirstOrDefaultAsync();

            int added = 0, sprIdx = sponsorStart + 1;

            // إضافة كفلاء جدد
            var newSponsors = new List<KafalaSponsor>();
            for (int k = sprIdx; k <= sponsorStart + 60; k++)
            {
                newSponsors.Add(new KafalaSponsor
                {
                    Id = Guid.NewGuid(),
                    SponsorCode = $"KSP-DEMO-{k:000}",
                    FullName = BuildName(k % 2 == 0),
                    SponsorType = "Individual",
                    NationalIdOrTaxNo = $"3010{k:000000000}",
                    PhoneNumber = $"0100{k:0000000}",
                    IsActive = true
                });
            }
            await db.Set<KafalaSponsor>().AddRangeAsync(newSponsors);
            await db.SaveChangesAsync();

            var allSponsors = await db.Set<KafalaSponsor>()
                .Where(x => x.SponsorCode.StartsWith("KSP-DEMO-"))
                .Select(x => x.Id).ToListAsync();

            var cases = new List<KafalaCase>();
            for (int k = existing + 1; k <= target && k - 1 < beneficiaryIds.Count; k++)
            {
                cases.Add(new KafalaCase
                {
                    Id = Guid.NewGuid(),
                    CaseNumber = $"KAF-DEMO-{k:0000}",
                    BeneficiaryId = beneficiaryIds[k - 1],
                    SponsorId = allSponsors[k % allSponsors.Count],
                    MonthlyAmount = 300m + (k % 12) * 50m,
                    StartDate = DateTime.Today.AddDays(-(60 + (k % 500))),
                    Status = k % 10 == 0 ? "Suspended" : k % 20 == 0 ? "Closed" : "Active",
                    PaymentMethodId = pmId == Guid.Empty ? null : pmId,
                  
                });

                if (cases.Count >= 200)
                {
                    await db.Set<KafalaCase>().AddRangeAsync(cases);
                    await db.SaveChangesAsync();
                    cases.Clear();
                }
            }
            if (cases.Any()) { await db.Set<KafalaCase>().AddRangeAsync(cases); await db.SaveChangesAsync(); }
            Console.WriteLine($"[Kafala] اكتملت الكفالات");
        }

        // ══════════════════════════════════════════════════
        //  3. المتبرعون والتبرعات — تكملة حتى 500 متبرع / 2000 تبرع
        // ══════════════════════════════════════════════════
        private static async Task SeedDonorsAndDonationsAsync(AppDbContext db, string? userId)
        {
            const int donorTarget = 500;
            const int donationTarget = 2_000;

            var existingDonors = await db.Set<Donor>()
                .CountAsync(x => x.Code.StartsWith("DON-DEMO-"));
            if (existingDonors < donorTarget)
            {
                var donors = new List<Donor>();
                for (int d = existingDonors + 1; d <= donorTarget; d++)
                {
                    donors.Add(new Donor
                    {
                        Id = Guid.NewGuid(),
                        Code = $"DON-DEMO-{d:000}",
                        FullName = d <= DonorNames.Length ? DonorNames[d - 1] : BuildName(d % 2 == 0),
                        DonorType = d % 7 == 0 ? "Company" : d % 11 == 0 ? "Institution" : "Individual",
                        PhoneNumber = $"0100{d:0000000}",
                        Email = $"donor{d:000}@demo.eg",
                        IsActive = true
                    });
                }
                await db.Set<Donor>().AddRangeAsync(donors);
                await db.SaveChangesAsync();
                Console.WriteLine($"[Donors] أُضيف {donors.Count} متبرع");
            }

            var existingDonations = await db.Set<Donation>()
                .CountAsync(x => x.DonationNumber.StartsWith("DN-DEMO-"));
            if (existingDonations >= donationTarget) return;

            var allDonorIds = await db.Set<Donor>()
                .Where(x => x.Code.StartsWith("DON-DEMO-"))
                .Select(x => x.Id).ToListAsync();

            var pmId = await db.Set<PaymentMethod>()
                .Where(x => x.IsActive && x.IsCash)
                .Select(x => x.Id).FirstOrDefaultAsync();

            // جيب حساب مالي للربط (أصل — بنك أو صندوق)
            var cashAccount = await InfrastructureManagmentDataAccess.EntityFramework.EF2
                .LargeScaleSeeder.GetCashAccountAsync(db);

            var donations = new List<Donation>();
            for (int d = existingDonations + 1; d <= donationTarget; d++)
            {
                var isCash = d % 5 != 0;
                donations.Add(new Donation
                {
                    Id = Guid.NewGuid(),
                    DonationNumber = $"DN-DEMO-{d:000}",
                    DonorId = allDonorIds[d % allDonorIds.Count],
                    DonationType = isCash ? "نقدي" : "غذائي",
                    TargetingScopeCode = isCash
                        ? (d % 3 == 0 ? "GeneralPurpose" : "GeneralFund")
                        : "SpecificRequests",
                    Amount = 500m + (d % 100) * 200m,
                    DonationDate = DateTime.Today.AddDays(-(d % 720)),
                    PaymentMethodId = pmId == Guid.Empty ? null : pmId,
                    FinancialAccountId = isCash ? cashAccount : null,
                    Notes = "تبرع ديمو",
                    
                    CreatedByUserId = userId
                });

                if (donations.Count >= 300)
                {
                    await db.Set<Donation>().AddRangeAsync(donations);
                    await db.SaveChangesAsync();
                    donations.Clear();
                }
            }
            if (donations.Any()) { await db.Set<Donation>().AddRangeAsync(donations); await db.SaveChangesAsync(); }
            Console.WriteLine($"[Donations] اكتملت التبرعات");
        }

        // ══════════════════════════════════════════════════
        //  4. اجتماعات إضافية — حتى 60 اجتماع
        // ══════════════════════════════════════════════════
        private static async Task SeedBoardMeetingsAsync(AppDbContext db, string? userId)
        {
            const int target = 60;
            var existing = await db.Set<BoardMeeting>()
                .CountAsync(x => x.MeetingNumber.StartsWith("MTG-DEMO-"));
            if (existing >= target) return;

            var meetings = new List<BoardMeeting>();
            var decisions = new List<BoardDecision>();

            for (int m = existing + 1; m <= target; m++)
            {
                var meetId = Guid.NewGuid();
                var date = DateTime.Today.AddDays(-(m * 30));
                meetings.Add(new BoardMeeting
                {
                    Id = meetId,
                    MeetingNumber = $"MTG-DEMO-{m:00}",
                    MeetingDate = date,
                    Title = $"اجتماع مجلس الإدارة رقم ({m}) لسنة {date.Year}",
                    Location = "مقر الجمعية",
                    Status = "Completed",
                    Agenda = $"1- مناقشة محضر الجلسة السابقة\n2- متابعة تنفيذ القرارات\n3- مناقشة الأعمال الجارية\n4- ما يستجد من أعمال",
                    IsActive = true
                });

                // 2-4 قرارات لكل اجتماع
                var decCount = 2 + (m % 3);
                for (int d = 1; d <= decCount; d++)
                {
                    decisions.Add(new BoardDecision
                    {
                        Id = Guid.NewGuid(),
                        BoardMeetingId = meetId,
                        DecisionNumber = $"{m}-{d}",
                        Title = _decisionTitles[(m * d) % _decisionTitles.Length],
                        Description = $"تم اتخاذ هذا القرار في اجتماع رقم {m} بتاريخ {date:d/M/yyyy}",
                        Status = d == 1 ? "Closed" : "InProgress",
                        DecisionKind = d % 3 == 0 ? "مالي" : d % 2 == 0 ? "إداري" : "تنفيذي",
                        
                    });
                }
            }

            await db.Set<BoardMeeting>().AddRangeAsync(meetings);
            await db.Set<BoardDecision>().AddRangeAsync(decisions);
            await db.SaveChangesAsync();
            Console.WriteLine($"[BoardMeetings] أُضيف {meetings.Count} اجتماع و{decisions.Count} قرار");
        }

        private static readonly string[] _decisionTitles = {
            "الموافقة على الميزانية السنوية",
            "اعتماد خطة العمل للربع القادم",
            "الموافقة على تعيين موظف جديد",
            "مناقشة وإقرار تقرير المتابعة الشهري",
            "الموافقة على فتح باب القبول للمستفيدين الجدد",
            "اعتماد صرف المساعدات الدورية",
            "مناقشة نتائج تقييم الأداء المؤسسي",
            "الموافقة على التعاقد مع جهة تمويل جديدة",
            "متابعة تنفيذ مشروع التنمية المجتمعية",
            "اعتماد تقرير المراجع الداخلي",
            "الموافقة على تجديد عقد التأمين",
            "مناقشة آلية توزيع الزكاة لهذا العام"
        };

        // ══════════════════════════════════════════════════
        //  5. مقترحات مشاريع — حتى 30 مقترح
        // ══════════════════════════════════════════════════
        private static async Task SeedProjectProposalsAsync(AppDbContext db, string? userId)
        {
            const int target = 30;
            var existing = await db.Set<ProjectProposal>()
                .CountAsync(x => true);
            if (existing >= target) return;

            var proposals = new List<ProjectProposal>();
            string[] titles = {
                "مشروع تمكين المرأة اقتصادياً", "مشروع دعم الأسر المتضررة", "مشروع الصرف الصحي الريفي",
                "مشروع محو الأمية الرقمية", "مشروع الأمن الغذائي المجتمعي", "مشروع دعم ذوي الإعاقة",
                "مشروع التشغيل للشباب", "مشروع الرعاية الصحية الأولية", "مشروع تدوير المخلفات",
                "مشروع الزراعة المستدامة", "مشروع الطاقة الشمسية للمدارس", "مشروع صندوق الطوارئ"
            };
            string[] funders = { "GIZ", "UNICEF", "UNDP", "الاتحاد الأوروبي", "USAID", "مؤسسة فورد", "البنك الدولي", "مؤسسة ساويرس", "UNHCR" };

            for (int p = existing + 1; p <= target; p++)
            {
                proposals.Add(new ProjectProposal
                {
                    Id = Guid.NewGuid(),
                  
                    Title = titles[p % titles.Length] + $" ({p:00})",
                    
                    RequestedBudget = 150_000m + (p % 30) * 50_000m,
                    Currency = "EGP",
                    DurationMonths = 6 + (p % 7),
                    ProjectLocation = Governorates[p % Governorates.Length],
                    Status = p % 5 == 0 ? "Approved" : p % 7 == 0 ? "Rejected" : "UnderReview",
                    SubmissionDate = DateTime.Today.AddDays(-(p * 15)),
                    GeneralGoal = $"تعزيز التنمية المستدامة وتحسين جودة الحياة في المجتمعات المستهدفة — المقترح {p}",
                    ExecutiveSummary = $"مقترح {titles[p % titles.Length]} يهدف إلى خدمة {500 + p * 100} مستفيد في محافظة {Governorates[p % Governorates.Length]}",
                    IsActive = true,
                    CreatedByUserId = userId
                });
            }

            await db.Set<ProjectProposal>().AddRangeAsync(proposals);
            await db.SaveChangesAsync();
            Console.WriteLine($"[ProjectProposals] أُضيف {proposals.Count} مقترح");
        }

        // ══════════════════════════════════════════════════
        //  6. متطوعون — حتى 300 متطوع
        // ══════════════════════════════════════════════════
        private static async Task SeedVolunteersAsync(AppDbContext db, string? userId)
        {
            const int target = 300;
            var existing = await db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Volunteers.Volunteer>()
                .CountAsync(x => x.VolunteerCode.StartsWith("VOL-DEMO-"));
            if (existing >= target) return;

            var vols = new List<InfrastrfuctureManagmentCore.Domains.Charity.Volunteers.Volunteer>();
            string[] skills = { "التدريب", "الطب", "المحاسبة", "القانون", "التصوير", "التسويق", "التعليم", "الهندسة", "تقنية المعلومات", "الخياطة" };

            for (int v = existing + 1; v <= target; v++)
            {
                vols.Add(new InfrastrfuctureManagmentCore.Domains.Charity.Volunteers.Volunteer
                {
                    Id = Guid.NewGuid(),
                    VolunteerCode = $"VOL-DEMO-{v:000}",
                    FullName = BuildName(v % 2 == 0),
                    PhoneNumber = $"0100{v:0000000}",
                   
                    IsActive = v % 12 != 0
                });
            }
            await db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Volunteers.Volunteer>()
                .AddRangeAsync(vols);
            await db.SaveChangesAsync();
            Console.WriteLine($"[Volunteers] أُضيف {vols.Count} متطوع");
        }

        // ══════════════════════════════════════════════════
        //  Helpers
        // ══════════════════════════════════════════════════
        private static async Task<StatusIds> LoadStatusIdsAsync(AppDbContext db)
        {
            var statuses = await db.Set<BeneficiaryStatusLookup>().ToListAsync();
            return new StatusIds
            {
                Approved = statuses.FirstOrDefault(x => x.NameAr.Contains("معتمد"))?.Id ?? Guid.Empty,
                Review = statuses.FirstOrDefault(x => x.NameAr.Contains("دراسة") || x.NameAr.Contains("مراجعة"))?.Id ?? Guid.Empty,
                New = statuses.FirstOrDefault(x => x.NameAr.Contains("جديد"))?.Id ?? Guid.Empty,
                Stopped = statuses.FirstOrDefault(x => x.NameAr.Contains("موقوف") || x.NameAr.Contains("متوقف"))?.Id ?? Guid.Empty,
            };
        }

        // ══════════════════════════════════════════════════
        //  موظفون إضافيون — حتى 50 موظف
        // ══════════════════════════════════════════════════
        private static async Task SeedEmployeesAsync(AppDbContext db, string? userId)
        {
            const int target = 50;
            var existing = await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrEmployee>()
                .CountAsync(x => x.Code.StartsWith("EMP-DEMO-"));
            if (existing >= target) return;

            var departments = await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrDepartment>()
                .Where(x => x.IsActive).ToListAsync();
            var jobTitles = await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle>()
                .Where(x => x.IsActive).ToListAsync();

            // أضف أقسام/وظائف لو مش موجودة
            if (!departments.Any())
            {
                var depts = new[] {
                    new InfrastrfuctureManagmentCore.Domains.HR.HrDepartment { Id=Guid.NewGuid(), Name="الإدارة العليا",       IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrDepartment { Id=Guid.NewGuid(), Name="الموارد البشرية",     IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrDepartment { Id=Guid.NewGuid(), Name="المستفيدين والاجتماعي",IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrDepartment { Id=Guid.NewGuid(), Name="المشروعات",           IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrDepartment { Id=Guid.NewGuid(), Name="المالية والمحاسبة",   IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrDepartment { Id=Guid.NewGuid(), Name="المخازن والمشتريات", IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrDepartment { Id=Guid.NewGuid(), Name="التسويق والتوعية",    IsActive=true },
                };
                await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrDepartment>().AddRangeAsync(depts);
                await db.SaveChangesAsync();
                departments = depts.ToList();
            }

            if (!jobTitles.Any())
            {
                var jobs = new[] {
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="مدير تنفيذي",          SystemRole="CharityManager",       IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="مدير مالي",            SystemRole="FinancialOfficer",     IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="مدير مشروعات",         SystemRole="ProjectManager",       IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="باحث اجتماعي",         SystemRole="SocialResearcher",     IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="أخصائي اجتماعي",       SystemRole="BeneficiariesOfficer", IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="محاسب",                 SystemRole="Accountant",           IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="مسؤول موارد بشرية",    SystemRole="HrOfficer",            IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="أمين مخزن",            SystemRole="StoreKeeper",          IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="مسؤول رواتب",          SystemRole="PayrollOfficer",       IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="منسق مشروع",           SystemRole="ProjectManager",       IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="مسؤول علاقات متبرعين", SystemRole="DonorRelations",       IsActive=true },
                    new InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle { Id=Guid.NewGuid(), Name="مراجع داخلي",          SystemRole="Reviewer",             IsActive=true },
                };
                await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrJobTitle>().AddRangeAsync(jobs);
                await db.SaveChangesAsync();
                jobTitles = jobs.ToList();
            }

            var shift = await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrShift>().FirstOrDefaultAsync();
            if (shift == null)
            {
                shift = new InfrastrfuctureManagmentCore.Domains.HR.HrShift
                {
                    Id = Guid.NewGuid(),
                    Name = "الوردية الصباحية",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0),
                    GraceMinutes = 15
                };
                await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrShift>().AddAsync(shift);
                await db.SaveChangesAsync();
            }

            var emps = new List<InfrastrfuctureManagmentCore.Domains.HR.HrEmployee>();
            var attendance = new List<InfrastrfuctureManagmentCore.Domains.HR.HrAttendanceRecord>();
            var leaveReqs = new List<InfrastrfuctureManagmentCore.Domains.HR.HrLeaveRequest>();
            var leaveTypes = await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrLeaveType>().ToListAsync();

            for (int i = existing + 1; i <= target; i++)
            {
                var male = i % 2 == 1;
                var dept = departments[(i - 1) % departments.Count];
                var job = jobTitles[(i - 1) % jobTitles.Count];
                var empId = Guid.NewGuid();

                var emp = new InfrastrfuctureManagmentCore.Domains.HR.HrEmployee
                {
                    Id = empId,
                    Code = $"EMP-DEMO-{i:000}",
                    FullName = BuildName(male),
                    NationalId = $"2980{i:000000000}",
                    BirthDate = DateTime.Today.AddYears(-(22 + (i % 18))),
                    PhoneNumber = $"0100{(1_000_000 + i % 8_999_999):0000000}",
                    Email = $"emp{i:000}@charity.local",
                    AddressLine = $"{Governorates[i % Governorates.Length]}",
                    DepartmentId = dept.Id,
                    JobTitleId = job.Id,
                    HireDate = DateTime.Today.AddMonths(-(6 + (i % 48))),
                    EmploymentType = i % 4 == 0 ? "Contract" : "Permanent",
                    BasicSalary = 4_000m + (i % 20) * 500m,
                    InsuranceSalary = 3_200m + (i % 20) * 400m,
                    BankName = "البنك الأهلي المصري",
                    BankAccountNumber = $"100200{i:000000}",
                    Status = i % 15 == 0 ? "OnLeave" : "Active",
                    IsActive = i % 20 != 0
                };
                emps.Add(emp);

                // سجل حضور آخر 14 يوم
                for (int d = 1; d <= 14; d++)
                {
                    var date = DateTime.Today.AddDays(-d);
                    if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday) continue;
                    if (Rng.Next(10) == 0) continue; // 10% غياب

                    attendance.Add(new InfrastrfuctureManagmentCore.Domains.HR.HrAttendanceRecord
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = empId,
                        AttendanceDate = date,
                        ShiftId = shift.Id,
                        CheckInTime = new TimeSpan(8, Rng.Next(0, 30), 0),
                        CheckOutTime = new TimeSpan(15, Rng.Next(0, 30), 0),
                        WorkedHours = 7m + (decimal)(Rng.NextDouble() * 1.5),
                        LateMinutes = Rng.Next(0, 15),
                        Status = "Present"
                    });
                }

                // إجازة واحدة لكل موظف في السنة (لو في leave types)
                if (leaveTypes.Any() && i % 3 != 0)
                {
                    var lt = leaveTypes[i % leaveTypes.Count];
                    var sDate = DateTime.Today.AddDays(-(10 + i % 60));
                    var days = 1 + (i % 5);
                    leaveReqs.Add(new InfrastrfuctureManagmentCore.Domains.HR.HrLeaveRequest
                    {
                        Id = Guid.NewGuid(),
                        RequestNumber = $"LVR-DEMO-{i:000}",
                        EmployeeId = empId,
                        LeaveTypeId = lt.Id,
                        StartDate = sDate,
                        EndDate = sDate.AddDays(days - 1),
                        TotalDays = days,
                        Status = i % 4 == 0 ? "Pending" : "Approved",
                        Reason = "إجازة ديمو للموظف",
                        CreatedByUserId = userId
                    });
                }
            }

            await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrEmployee>().AddRangeAsync(emps);
            await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrAttendanceRecord>().AddRangeAsync(attendance);
            if (leaveReqs.Any())
                await db.Set<InfrastrfuctureManagmentCore.Domains.HR.HrLeaveRequest>().AddRangeAsync(leaveReqs);
            await db.SaveChangesAsync();
            Console.WriteLine($"[Employees] أُضيف {emps.Count} موظف · {attendance.Count} سجل حضور · {leaveReqs.Count} إجازة");
        }

   

        // جيب ID حساب الأصول النقدي
        public static async Task<Guid?> GetCashAccountAsync(AppDbContext db)
        {
            var acc = await db.Set<InfrastrfuctureManagmentCore.Domains.Financial.FinancialAccount>()
                .Where(x => x.IsPosting &&
                       (x.Category == InfrastrfuctureManagmentCore.Domains.Financial.AccountCategory.Asset))
                .OrderBy(x => x.AccountCode)
                .FirstOrDefaultAsync();
            return acc?.Id;
        }

        private record StatusIds
        {
            public Guid Approved { get; init; }
            public Guid Review { get; init; }
            public Guid New { get; init; }
            public Guid Stopped { get; init; }
        }
    }
}
