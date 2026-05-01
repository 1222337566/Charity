using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    /// <summary>
    /// Seed مشروع جرين فيجين (Green Vision) — جمعية تواصل سوهاج
    /// مبني على المقترح الفني المقدم لـ GIZ / NSWMP
    /// </summary>
    public static class GreenVisionSeeder
    {
        // ── IDs ثابتة لضمان idempotency ──
        private static readonly Guid ProjectId   = new("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
        private static readonly Guid Phase1Id    = new("b1000001-0000-0000-0000-000000000001");
        private static readonly Guid Phase2Id    = new("b1000002-0000-0000-0000-000000000002");
        private static readonly Guid Phase3Id    = new("b1000003-0000-0000-0000-000000000003");
        private static readonly Guid Phase4Id    = new("b1000004-0000-0000-0000-000000000004");
        private static readonly Guid GoalMainId  = new("c1000001-0000-0000-0000-000000000001");
        private static readonly Guid Sub1Id      = new("d1000001-0000-0000-0000-000000000001");
        private static readonly Guid Sub2Id      = new("d1000002-0000-0000-0000-000000000002");
        private static readonly Guid Sub3Id      = new("d1000003-0000-0000-0000-000000000003");
        private static readonly Guid Sub4Id      = new("d1000004-0000-0000-0000-000000000004");

        public static async Task SeedAsync(AppDbContext db)
        {
            // Idempotent — لو المشروع موجود، لا تُكرر
            if (await db.Set<CharityProject>().AnyAsync(x => x.Id == ProjectId))
                return;

            var today = DateTime.Today;

            // ════════════════════════════════════════════════════════
            //  1. المشروع الرئيسي
            // ════════════════════════════════════════════════════════
            var project = new CharityProject
            {
                Id          = ProjectId,
                Code        = "PP-GRN-001",
                Name        = "جرين فيجين — إدارة وتدوير مخلفات أشجار الموز",
                Description = "مشروع لجمع وإعادة تدوير مخلفات أشجار الموز وتحويلها إلى منتجات متنوعة " +
                              "(ورق، أطباق، اكسسوارات، هدايا، شنط) بالقرى المستهدفة بأسيوط. " +
                              "ممول من الوكالة الألمانية للتعاون الدولي (GIZ) ضمن برنامج NSWMP/EU Green.",
                StartDate   = today,
                EndDate     = today.AddMonths(8),
                Budget      = 2_348_271m,
                Status      = "Active",
                Location    = "قرى باقور، المطيعة، الشغبة، أولاد إبراهيم — مركز أبوتيج، أسيوط",
                Objectives  = "تعزيز الاستدامة البيئية والاقتصادية للحد من التلوث البيئي وخلق فرص عمل مستدامة",
                Kpis        = "200 شخص مستفيد من التوعية · 60 شاب/فتاه مدرب · 15 فرصة عمل · وحدة إنتاجية واحدة",
                TargetBeneficiariesCount = 310,
                IsActive    = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            db.Set<CharityProject>().Add(project);

            // ════════════════════════════════════════════════════════
            //  2. المراحل الأربع (8 أشهر)
            // ════════════════════════════════════════════════════════
            var phases = new List<ProjectPhase>
            {
                new() { Id = Phase1Id, ProjectId = ProjectId, SortOrder = 1,
                    Code = "M1-2",  Name = "المرحلة الأولى — التحضير والتوعية",
                    PlannedStartDate = today,           PlannedEndDate = today.AddMonths(2),
                    Status = "Active", ProgressPercent = 0,
                    PlannedCost = 350_000, ResponsiblePersonName = "مشرف + منسق + مسؤول المتابعة",
                    IsActive = true },

                new() { Id = Phase2Id, ProjectId = ProjectId, SortOrder = 2,
                    Code = "M2-4",  Name = "المرحلة الثانية — التدريب والتأهيل",
                    PlannedStartDate = today.AddMonths(2), PlannedEndDate = today.AddMonths(4),
                    Status = "Planned", ProgressPercent = 0,
                    PlannedCost = 520_000, ResponsiblePersonName = "مشرف + منسق + مدرب",
                    IsActive = true },

                new() { Id = Phase3Id, ProjectId = ProjectId, SortOrder = 3,
                    Code = "M3-5",  Name = "المرحلة الثالثة — الشراكات والتنسيق",
                    PlannedStartDate = today.AddMonths(3), PlannedEndDate = today.AddMonths(5),
                    Status = "Planned", ProgressPercent = 0,
                    PlannedCost = 180_000, ResponsiblePersonName = "مدير مشروع + استشاري",
                    IsActive = true },

                new() { Id = Phase4Id, ProjectId = ProjectId, SortOrder = 4,
                    Code = "M4-8",  Name = "المرحلة الرابعة — التشغيل والتسويق",
                    PlannedStartDate = today.AddMonths(4), PlannedEndDate = today.AddMonths(8),
                    Status = "Planned", ProgressPercent = 0,
                    PlannedCost = 1_298_271, ResponsiblePersonName = "مدير مشروع + مسؤول تسويق",
                    IsActive = true },
            };
            db.Set<ProjectPhase>().AddRange(phases);
            await db.SaveChangesAsync(); // ← حفظ المشروع والمراحل أولاً

            // ════════════════════════════════════════════════════════
            //  3. الهدف الرئيسي
            // ════════════════════════════════════════════════════════
            var mainGoal = new ProjectGoal
            {
                Id = GoalMainId, ProjectId = ProjectId, SortOrder = 1,
                Title = "تعزيز الاستدامة البيئية والاقتصادية بالمجتمعات المستهدفة",
                Description = "الحد من التلوث البيئي الناتج عن إهدار 45 ألف طن سنوياً من مخلفات " +
                              "أشجار الموز وتحويلها لفرص اقتصادية",
                SuccessIndicator = "نسبة تخفيض حرق/إلقاء المخلفات",
                TargetValue = "تخفيض 50% من المخلفات المُهدَرة",
                Status = "Active", IsActive = true
            };
            db.Set<ProjectGoal>().Add(mainGoal);
            await db.SaveChangesAsync(); // ← حفظ الهدف الرئيسي

            // ════════════════════════════════════════════════════════
            //  4. الأهداف الفرعية + الأنشطة
            // ════════════════════════════════════════════════════════

            // ── الهدف الفرعي 1: رفع الوعي ──
            var sub1 = new ProjectSubGoal
            {
                Id = Sub1Id, ProjectId = ProjectId, GoalId = GoalMainId, SortOrder = 1,
                Title = "رفع الوعي لعدد 200 شخص بأهمية جمع وتدوير مخلفات أشجار الموز",
                SuccessIndicator = "نسبة وعدد حضور الندوات",
                TargetValue = "200 شخص / 100%",
                Status = "Active", IsActive = true
            };
            db.Set<ProjectSubGoal>().Add(sub1);
            await db.SaveChangesAsync();

            var act1_1Id = Guid.NewGuid();
            var act1_1 = new ProjectSubGoalActivity
            {
                Id = act1_1Id, ProjectId = ProjectId, SubGoalId = Sub1Id, SortOrder = 1,
                Title = "تنفيذ 4 ندوات توعية مجتمعية حول جمع وتدوير مخلفات الموز",
                Description = "تنفيذ عدد 4 ندوة توعية لعدد 200 شخص لرفع الوعي المجتمعي " +
                              "بأهمية جمع وتدوير مخلفات أشجار الموز والنخيل",
                TargetGroup = "مزارعون وشباب وفتيات (20-60 سنة)",
                PlannedQuantity = 200, QuantityUnit = "شخص",
                PlannedDurationDays = 4, PlannedHoursPerDay = 3,
                PlannedCost = 85_000, Priority = "High",
                PerformanceIndicator = "نسبة وعدد حضور الندوات",
                VerificationMeans = "كشف حضور الندوات — صور وفيديوهات — فواتير",
                TargetAchievement = "100%",
                ResponsiblePersonName = "مشرف + منسق + مسؤول المتابعة",
                Status = "Planned", IsActive = true
            };
            db.Set<ProjectSubGoalActivity>().Add(act1_1);
            await db.SaveChangesAsync();

            // الندوات تبدأ من الشهر 1 إلى الشهر 2 (المرحلة الأولى بالكامل)
            db.Set<ActivityPhaseAssignment>().Add(new ActivityPhaseAssignment
            {
                Id = Guid.NewGuid(), ActivityId = act1_1Id, PhaseId = Phase1Id,
                SortOrder = 1, PlannedQuantity = 200, ContributionPercent = 100,
                PlannedDurationDays = 4, PlannedHoursPerDay = 3, PlannedCost = 85_000,
                PlannedStartDate = today.AddDays(14),
                PlannedEndDate   = today.AddMonths(2),
                Status = "Planned"
            });

            // ── الهدف الفرعي 2: بناء القدرات ──
            var sub2 = new ProjectSubGoal
            {
                Id = Sub2Id, ProjectId = ProjectId, GoalId = GoalMainId, SortOrder = 2,
                Title = "بناء قدرات ومهارات عدد 60 شاب/فتاه على إنتاج منتجات متنوعة من مخلفات الموز",
                SuccessIndicator = "عدد الورشات المنفذة — عدد المشاركين — نتيجة الاختبار",
                TargetValue = "60 شاب/فتاه / 90%",
                Status = "Active", IsActive = true
            };
            db.Set<ProjectSubGoal>().Add(sub2);
            await db.SaveChangesAsync();

            // النشاط 2-1: 3 ورشات تدريبية
            var act2_1Id = Guid.NewGuid();
            var act2_1 = new ProjectSubGoalActivity
            {
                Id = act2_1Id, ProjectId = ProjectId, SubGoalId = Sub2Id, SortOrder = 1,
                Title = "تنفيذ 3 ورشات تدريبية لإنتاج منتجات متنوعة من مخلفات الموز",
                Description = "تنفيذ عدد 3 ورشات تدريبية لعدد 60 شاب/فتاه بواقع 20 شاب/فتاه " +
                              "لكل ورشة لمدة 8 أيام بواقع 5 ساعات يومياً على إنتاج منتجات متنوعة",
                TargetGroup = "شباب وفتيات (19-40 سنة) — 10 شباب + 50 فتاة",
                PlannedQuantity = 60, QuantityUnit = "متدرب",
                PlannedDurationDays = 24, PlannedHoursPerDay = 5,  // 3 ورشات × 8 أيام
                PlannedCost = 320_000, Priority = "High",
                PerformanceIndicator = "عدد الورشات — عدد المشاركين — نتيجة الاختبار القبلي والبعدي",
                VerificationMeans = "كشف الحضور — استمارة التقييم — تقرير الأنشطة — صور وفيديوهات",
                TargetAchievement = "90%",
                ResponsiblePersonName = "مشرف + منسق + مسؤول المتابعة",
                Status = "Planned", IsActive = true
            };
            db.Set<ProjectSubGoalActivity>().Add(act2_1);
            await db.SaveChangesAsync();

            // الورشة الأولى: المرحلة الثانية
            db.Set<ActivityPhaseAssignment>().Add(new ActivityPhaseAssignment
            {
                Id = Guid.NewGuid(), ActivityId = act2_1Id, PhaseId = Phase2Id,
                SortOrder = 1, PlannedQuantity = 40, ContributionPercent = 67,
                PlannedDurationDays = 16, PlannedHoursPerDay = 5, PlannedCost = 213_000,
                PlannedStartDate = today.AddMonths(2).AddDays(7),
                PlannedEndDate   = today.AddMonths(3).AddDays(15),
                Status = "Planned"
            });
            // الورشة الثالثة: المرحلة الثالثة
            db.Set<ActivityPhaseAssignment>().Add(new ActivityPhaseAssignment
            {
                Id = Guid.NewGuid(), ActivityId = act2_1Id, PhaseId = Phase3Id,
                SortOrder = 2, PlannedQuantity = 20, ContributionPercent = 33,
                PlannedDurationDays = 8, PlannedHoursPerDay = 5, PlannedCost = 107_000,
                PlannedStartDate = today.AddMonths(4),
                PlannedEndDate   = today.AddMonths(4).AddDays(10),
                Status = "Planned"
            });

            // النشاط 2-2: ورشة متقدمة لإعداد مدربين
            var act2_2Id = Guid.NewGuid();
            var act2_2 = new ProjectSubGoalActivity
            {
                Id = act2_2Id, ProjectId = ProjectId, SubGoalId = Sub2Id, SortOrder = 2,
                Title = "ورشة تدريبية متقدمة لإعداد 25 مدرباً محترفاً",
                Description = "تنفيذ عدد 1 ورشة تدريبية متقدمة لعدد 25 متدرب/ة لمدة 5 أيام " +
                              "بواقع 4 ساعات يومياً لإعداد مدربين محترفين",
                TargetGroup = "الكوادر المدربة مسبقاً من الدورات الثلاث",
                PlannedQuantity = 25, QuantityUnit = "متدرب",
                PlannedDurationDays = 5, PlannedHoursPerDay = 4,
                PlannedCost = 95_000, Priority = "Medium",
                PerformanceIndicator = "عدد المشاركين — نتيجة الاختبار — مواد علمية",
                VerificationMeans = "كشف الحضور — تقرير المتابعة — صور وفيديوهات",
                TargetAchievement = "90%",
                ResponsiblePersonName = "مشرف + منسق + مسؤول المتابعة",
                Status = "Planned", IsActive = true
            };
            db.Set<ProjectSubGoalActivity>().Add(act2_2);
            await db.SaveChangesAsync();

            db.Set<ActivityPhaseAssignment>().Add(new ActivityPhaseAssignment
            {
                Id = Guid.NewGuid(), ActivityId = act2_2Id, PhaseId = Phase2Id,
                SortOrder = 1, PlannedQuantity = 25, ContributionPercent = 100,
                PlannedDurationDays = 5, PlannedHoursPerDay = 4, PlannedCost = 95_000,
                PlannedStartDate = today.AddMonths(3).AddDays(20),
                PlannedEndDate   = today.AddMonths(4),
                Status = "Planned"
            });

            // ── الهدف الفرعي 3: الشراكات ──
            var sub3 = new ProjectSubGoal
            {
                Id = Sub3Id, ProjectId = ProjectId, GoalId = GoalMainId, SortOrder = 3,
                Title = "خلق شراكات مع 50 جهة حكومية وغير حكومية لدعم أنشطة المشروع",
                SuccessIndicator = "عدد بروتوكولات التعاون — قرارات الموائد",
                TargetValue = "5 بروتوكولات تعاون / 50 قيادة",
                Status = "Active", IsActive = true
            };
            db.Set<ProjectSubGoal>().Add(sub3);
            await db.SaveChangesAsync();

            // النشاط 3-1: موائد مستديرة
            var act3_1Id = Guid.NewGuid();
            var act3_1 = new ProjectSubGoalActivity
            {
                Id = act3_1Id, ProjectId = ProjectId, SubGoalId = Sub3Id, SortOrder = 1,
                Title = "عقد 2 مائدة مستديرة لـ 50 قيادة حكومية وغير حكومية",
                Description = "عقد عدد 2 مائدة مستديرة لعدد 50 قيادة حكومية وغير حكومية " +
                              "بواقع 25 قيادة بكل مائدة لدعم أنشطة المشروع",
                TargetGroup = "قيادات حكومية ومنظمات مجتمع مدني مهتمة بالبيئة والزراعة",
                PlannedQuantity = 50, QuantityUnit = "قيادة",
                PlannedDurationDays = 2, PlannedHoursPerDay = 4,
                PlannedCost = 55_000, Priority = "High",
                PerformanceIndicator = "عدد الموائد — عدد المشاركين — التوصيات والقرارات",
                VerificationMeans = "كشف الحضور — تقرير المائدة — صور وفيديوهات",
                TargetAchievement = "100%",
                ResponsiblePersonName = "مشرف منطقة + منسق المحافظة + مسؤول التدريب",
                Status = "Planned", IsActive = true
            };
            db.Set<ProjectSubGoalActivity>().Add(act3_1);
            await db.SaveChangesAsync();

            db.Set<ActivityPhaseAssignment>().AddRange(
                new ActivityPhaseAssignment
                {
                    Id = Guid.NewGuid(), ActivityId = act3_1Id, PhaseId = Phase1Id,
                    SortOrder = 1, PlannedQuantity = 25, ContributionPercent = 50,
                    PlannedDurationDays = 1, PlannedHoursPerDay = 4, PlannedCost = 27_500,
                    PlannedStartDate = today.AddMonths(1),
                    PlannedEndDate   = today.AddMonths(1).AddDays(2),
                    Status = "Planned"
                },
                new ActivityPhaseAssignment
                {
                    Id = Guid.NewGuid(), ActivityId = act3_1Id, PhaseId = Phase3Id,
                    SortOrder = 2, PlannedQuantity = 25, ContributionPercent = 50,
                    PlannedDurationDays = 1, PlannedHoursPerDay = 4, PlannedCost = 27_500,
                    PlannedStartDate = today.AddMonths(4),
                    PlannedEndDate   = today.AddMonths(4).AddDays(2),
                    Status = "Planned"
                }
            );

            // النشاط 3-2: إنشاء لجان تنسيقية
            var act3_2Id = Guid.NewGuid();
            var act3_2 = new ProjectSubGoalActivity
            {
                Id = act3_2Id, ProjectId = ProjectId, SubGoalId = Sub3Id, SortOrder = 2,
                Title = "إنشاء وتشكيل لجان تنسيقية من كبار مزارعي الموز والجهات المعنية",
                TargetGroup = "كبار المزارعين + ممثلو الجهات الإدارية + منظمات المجتمع المدني",
                PlannedQuantity = 1, QuantityUnit = "لجنة",
                PlannedDurationDays = 3, PlannedCost = 25_000, Priority = "Medium",
                PerformanceIndicator = "عدد اللجان المشكّلة — قوائم الأعضاء",
                VerificationMeans = "محاضر الاجتماعات — قوائم الأعضاء — صور",
                TargetAchievement = "100%",
                ResponsiblePersonName = "مدير مشروع + استشاري + منسق",
                Status = "Planned", IsActive = true
            };
            db.Set<ProjectSubGoalActivity>().Add(act3_2);
            await db.SaveChangesAsync();

            db.Set<ActivityPhaseAssignment>().Add(new ActivityPhaseAssignment
            {
                Id = Guid.NewGuid(), ActivityId = act3_2Id, PhaseId = Phase1Id,
                SortOrder = 1, PlannedQuantity = 1, ContributionPercent = 100,
                PlannedDurationDays = 3, PlannedCost = 25_000,
                PlannedStartDate = today.AddMonths(1).AddDays(7),
                PlannedEndDate   = today.AddMonths(1).AddDays(10),
                Status = "Planned"
            });

            // النشاط 3-3: لقاءات شهرية
            var act3_3Id = Guid.NewGuid();
            var act3_3 = new ProjectSubGoalActivity
            {
                Id = act3_3Id, ProjectId = ProjectId, SubGoalId = Sub3Id, SortOrder = 3,
                Title = "عقد 20 لقاء شهري للجان التنسيقية لمتابعة أنشطة المشروع",
                TargetGroup = "أعضاء اللجان التنسيقية",
                PlannedQuantity = 20, QuantityUnit = "لقاء",
                PlannedDurationDays = 20, PlannedHoursPerDay = 2,
                PlannedCost = 40_000, Priority = "Medium",
                PerformanceIndicator = "عدد اللقاءات الشهرية — عدد المشاركين",
                VerificationMeans = "محاضر الاجتماعات — كشف الحضور — صور",
                TargetAchievement = "100%",
                ResponsiblePersonName = "مدير المشروع + منسق + مشرف",
                Status = "Planned", IsActive = true
            };
            db.Set<ProjectSubGoalActivity>().Add(act3_3);
            await db.SaveChangesAsync();

            // اللقاءات على 4 مراحل (5 لقاءات × 4 مراحل)
            foreach (var (phId, from, to) in new[] {
                (Phase1Id, today.AddMonths(1), today.AddMonths(2)),
                (Phase2Id, today.AddMonths(2), today.AddMonths(4)),
                (Phase3Id, today.AddMonths(4), today.AddMonths(5)),
                (Phase4Id, today.AddMonths(5), today.AddMonths(8)) })
            {
                db.Set<ActivityPhaseAssignment>().Add(new ActivityPhaseAssignment
                {
                    Id = Guid.NewGuid(), ActivityId = act3_3Id, PhaseId = phId,
                    SortOrder = 1, PlannedQuantity = 5, ContributionPercent = 25,
                    PlannedDurationDays = 5, PlannedHoursPerDay = 2, PlannedCost = 10_000,
                    PlannedStartDate = from, PlannedEndDate = to,
                    Status = "Planned"
                });
            }

            // ── الهدف الفرعي 4: الوحدة الإنتاجية ──
            var sub4 = new ProjectSubGoal
            {
                Id = Sub4Id, ProjectId = ProjectId, GoalId = GoalMainId, SortOrder = 4,
                Title = "إنشاء وتفعيل وحدة تأهيلية إنتاجية لجمع وتدوير مخلفات أشجار الموز",
                SuccessIndicator = "عدد الوحدات المجهزة — كمية المنتجات — عدد العاملين",
                TargetValue = "1 وحدة إنتاجية / 15 فرصة عمل",
                Status = "Active", IsActive = true
            };
            db.Set<ProjectSubGoal>().Add(sub4);
            await db.SaveChangesAsync();

            // النشاط 4-1: تجهيز الوحدة الإنتاجية
            var act4_1Id = Guid.NewGuid();
            var act4_1 = new ProjectSubGoalActivity
            {
                Id = act4_1Id, ProjectId = ProjectId, SubGoalId = Sub4Id, SortOrder = 1,
                Title = "تجهيز وتأسيس وحدة إنتاجية تأهيلية لإنتاج وبيع المنتجات المتنوعة",
                Description = "تجهيز وتأسيس وحدة الإنتاج وبيع المنتجات المتنوعة على أن يكون " +
                              "المتدربون هم نواة التشغيل لتلك الوحدة",
                TargetGroup = "15 شاب/فتاه من أكفء العناصر المدربة",
                PlannedQuantity = 1, QuantityUnit = "وحدة إنتاجية",
                PlannedCost = 850_000, Priority = "Critical",
                PerformanceIndicator = "عدد الوحدات الإنتاجية — كمية المنتجات — عدد العاملين",
                VerificationMeans = "بيان الإنتاج الشهري — فواتير الآلات — صور الورشة",
                TargetAchievement = "100%",
                ResponsiblePersonName = "مدير مشروع + محاسب + منسق + مشرف",
                Status = "Planned", IsActive = true
            };
            db.Set<ProjectSubGoalActivity>().Add(act4_1);
            await db.SaveChangesAsync();

            db.Set<ActivityPhaseAssignment>().AddRange(
                // تجهيز أولي في المرحلة الثانية
                new ActivityPhaseAssignment
                {
                    Id = Guid.NewGuid(), ActivityId = act4_1Id, PhaseId = Phase2Id,
                    SortOrder = 1, PlannedQuantity = 0, ContributionPercent = 30,
                    PlannedCost = 255_000,
                    PlannedStartDate = today.AddMonths(2),
                    PlannedEndDate   = today.AddMonths(4),
                    Status = "Planned", Notes = "شراء الآلات والمعدات"
                },
                // تشغيل كامل في المرحلة الرابعة
                new ActivityPhaseAssignment
                {
                    Id = Guid.NewGuid(), ActivityId = act4_1Id, PhaseId = Phase4Id,
                    SortOrder = 2, PlannedQuantity = 1, ContributionPercent = 70,
                    PlannedCost = 595_000,
                    PlannedStartDate = today.AddMonths(4),
                    PlannedEndDate   = today.AddMonths(8),
                    Status = "Planned", Notes = "تشغيل وإنتاج وتوظيف"
                }
            );

            // النشاط 4-2: منصة إلكترونية
            var act4_2Id = Guid.NewGuid();
            var act4_2 = new ProjectSubGoalActivity
            {
                Id = act4_2Id, ProjectId = ProjectId, SubGoalId = Sub4Id, SortOrder = 2,
                Title = "تطوير منصة إلكترونية لعرض وبيع المنتجات المتنوعة",
                TargetGroup = "المستهلكون المحليون والدوليون",
                PlannedQuantity = 1, QuantityUnit = "منصة",
                PlannedCost = 75_000, Priority = "Medium",
                PerformanceIndicator = "عدد المنتجات المعروضة — عدد الزيارات — نسبة المبيعات",
                VerificationMeans = "تقرير تطوير المنصة — بيان التحليلات — فواتير",
                TargetAchievement = "100%",
                ResponsiblePersonName = "مدير المشروع + مسؤول تسويق",
                Status = "Planned", IsActive = true
            };
            db.Set<ProjectSubGoalActivity>().Add(act4_2);
            await db.SaveChangesAsync();

            db.Set<ActivityPhaseAssignment>().Add(new ActivityPhaseAssignment
            {
                Id = Guid.NewGuid(), ActivityId = act4_2Id, PhaseId = Phase4Id,
                SortOrder = 1, PlannedQuantity = 1, ContributionPercent = 100,
                PlannedCost = 75_000,
                PlannedStartDate = today.AddMonths(5),
                PlannedEndDate   = today.AddMonths(6),
                Status = "Planned"
            });

            // النشاط 4-3: المشاركة بالمعارض
            var act4_3Id = Guid.NewGuid();
            var act4_3 = new ProjectSubGoalActivity
            {
                Id = act4_3Id, ProjectId = ProjectId, SubGoalId = Sub4Id, SortOrder = 3,
                Title = "المشاركة في معرض محلي/دولي لترويج المنتجات المعاد تدويرها",
                TargetGroup = "زوار المعارض والمستثمرون",
                PlannedQuantity = 1, QuantityUnit = "معرض",
                PlannedCost = 45_000, Priority = "Medium",
                PerformanceIndicator = "عدد المنتجات المباعة — عدد المعارض — عدد الزوار",
                VerificationMeans = "تقرير المشاركة — فواتير البيع — صور وفيديوهات",
                TargetAchievement = "100%",
                ResponsiblePersonName = "مسؤول التسويق",
                Status = "Planned", IsActive = true
            };
            db.Set<ProjectSubGoalActivity>().Add(act4_3);
            await db.SaveChangesAsync();

            db.Set<ActivityPhaseAssignment>().Add(new ActivityPhaseAssignment
            {
                Id = Guid.NewGuid(), ActivityId = act4_3Id, PhaseId = Phase4Id,
                SortOrder = 1, PlannedQuantity = 1, ContributionPercent = 100,
                PlannedCost = 45_000,
                PlannedStartDate = today.AddMonths(7),
                PlannedEndDate   = today.AddMonths(7).AddDays(15),
                Status = "Planned"
            });

            // ════════════════════════════════════════════════════════
            //  5. بنود الميزانية
            // ════════════════════════════════════════════════════════
            var budgetLines = new List<ProjectBudgetLine>
            {
                new() { Id=Guid.NewGuid(), ProjectId=ProjectId, LineName="أنشطة التوعية والإعلام",
                    LineType="Operations", PlannedAmount=150_271, ActualAmount=0 },
                new() { Id=Guid.NewGuid(), ProjectId=ProjectId, LineName="التدريب وبناء القدرات",
                    LineType="Training",   PlannedAmount=415_000, ActualAmount=0 },
                new() { Id=Guid.NewGuid(), ProjectId=ProjectId, LineName="تجهيز الوحدة الإنتاجية",
                    LineType="Equipment",  PlannedAmount=850_000, ActualAmount=0 },
                new() { Id=Guid.NewGuid(), ProjectId=ProjectId, LineName="الشراكات والموائد المستديرة",
                    LineType="Operations", PlannedAmount=120_000, ActualAmount=0 },
                new() { Id=Guid.NewGuid(), ProjectId=ProjectId, LineName="المنصة الإلكترونية والتسويق",
                    LineType="Marketing",  PlannedAmount=120_000, ActualAmount=0 },
                new() { Id=Guid.NewGuid(), ProjectId=ProjectId, LineName="الرواتب والموارد البشرية",
                    LineType="HR",         PlannedAmount=480_000, ActualAmount=0 },
                new() { Id=Guid.NewGuid(), ProjectId=ProjectId, LineName="المتابعة والتقييم والتقارير",
                    LineType="M&E",        PlannedAmount=120_000, ActualAmount=0 },
                new() { Id=Guid.NewGuid(), ProjectId=ProjectId, LineName="الإدارة والتشغيل",
                    LineType="Admin",      PlannedAmount=113_000, ActualAmount=0 },
            };
            db.Set<ProjectBudgetLine>().AddRange(budgetLines);

            // ════════════════════════════════════════════════════════
            //  6. مستفيدو المشروع (عينة)
            // ════════════════════════════════════════════════════════
            await SeedProjectBeneficiariesAsync(db, ProjectId);

            await db.SaveChangesAsync();
        }

        private static async Task SeedProjectBeneficiariesAsync(AppDbContext db, Guid projectId)
        {
            // أضف 15 مستفيداً كعينة ديمو
            var villages = new[] { "باقور", "المطيعة", "الشغبة", "أولاد إبراهيم" };
            var benefitTypes = new[] { "تدريب إنتاجي", "توعية بيئية", "توعية + تدريب" };
            var rng = new Random(42);

            for (int i = 1; i <= 15; i++)
            {
                var benId = Guid.NewGuid();
                var isFemale = i % 3 != 0;
                var name = isFemale
                    ? $"{new[]{"فاطمة","أسماء","مريم","رحاب","هدى","سارة","نهى","إيمان","دعاء","ولاء"}[i % 10]} {new[]{"حسن","علي","السيد","مبارك","طه"}[i % 5]}"
                    : $"{new[]{"أحمد","محمد","خالد","مصطفى","طارق","هيثم","وائل"}[i % 7]} {new[]{"عبدالسلام","محمد","النوبي","صالح"}[i % 4]}";

                var ben = new Beneficiary
                {
                    Id = benId,
                    Code = $"BN-GRN-{i:000}",
                    FullName = name,
                    NationalId = $"2{rng.Next(100000000, 999999999):000000000}",
                    PhoneNumber = $"01{rng.Next(0, 4)}{rng.Next(10000000, 99999999)}",
                    FamilyMembersCount = rng.Next(3, 8),
                    MonthlyIncome = rng.Next(0, 3) == 0 ? 0 : rng.Next(500, 2500),
                    RegistrationDate = DateTime.Today,
                    Location = villages[i % villages.Length],
                    WorkStatus = i % 4 == 0 ? "عاطل" : "مزارع",
                    EducationStatus = i % 3 == 0 ? "إعدادي" : "ابتدائي",
                    IsActive = true
                };
                db.Set<Beneficiary>().Add(ben);

                db.Set<ProjectBeneficiary>().Add(new ProjectBeneficiary
                {
                    Id = Guid.NewGuid(), ProjectId = projectId, BeneficiaryId = benId,
                    EnrollmentDate = DateTime.Today,
                    BenefitType = benefitTypes[i % benefitTypes.Length]
                });
            }
        }
    }
}
