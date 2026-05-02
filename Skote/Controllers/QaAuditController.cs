using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.ViewModels.QaAudit;

namespace Skote.Controllers
{
    [Authorize]
    public class QaAuditController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public QaAuditController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = BuildModel();
            return View(model);
        }

        [HttpGet]
        public IActionResult Routes()
        {
            var model = BuildModel();
            return View(model);
        }

        [HttpGet]
        public IActionResult Workflows()
        {
            var model = BuildModel();
            return View(model);
        }

        private QaAuditIndexVm BuildModel()
        {
            var model = new QaAuditIndexVm
            {
                GeneratedAtUtc = DateTime.UtcNow,
                Workflows = BuildWorkflows()
            };

            var assembly = typeof(QaAuditController).Assembly;
            var controllerTypes = assembly
                .GetTypes()
                .Where(t => typeof(Controller).IsAssignableFrom(t)
                            && t.IsClass
                            && !t.IsAbstract
                            && t.Name.EndsWith("Controller", StringComparison.Ordinal)
                            && !t.Name.StartsWith("QaAudit", StringComparison.Ordinal))
                .OrderBy(t => t.Name)
                .ToList();

            foreach (var controllerType in controllerTypes)
            {
                var controllerName = controllerType.Name[..^"Controller".Length];
                var controllerVm = new QaControllerSummaryVm
                {
                    ControllerName = controllerName
                };

                var methods = controllerType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(IsMvcAction)
                    .OrderBy(m => m.Name)
                    .ToList();

                foreach (var method in methods)
                {
                    var actionName = method.GetCustomAttribute<ActionNameAttribute>()?.Name ?? method.Name;
                    var parameters = method.GetParameters()
                        .Where(p => p.ParameterType != typeof(CancellationToken))
                        .ToList();
                    var requiresParameters = parameters.Any(p => !IsInfrastructureParameter(p.ParameterType));
                    var viewPath = Path.Combine(_environment.ContentRootPath, "Views", controllerName, actionName + ".cshtml");
                    var isHttpPost = method.GetCustomAttributes<HttpPostAttribute>(true).Any();
                    var isHttpGet = !isHttpPost || method.GetCustomAttributes<HttpGetAttribute>(true).Any();
                    var suggestedUrl = BuildSuggestedUrl(controllerName, actionName, requiresParameters);

                    var item = new QaActionSummaryVm
                    {
                        ControllerName = controllerName,
                        ActionName = actionName,
                        SuggestedUrl = suggestedUrl,
                        IsHttpGet = isHttpGet,
                        IsHttpPost = isHttpPost,
                        RequiresParameters = requiresParameters,
                        ParametersDescription = string.Join(", ", parameters.Select(p => $"{p.Name}: {p.ParameterType.Name}")),
                        ViewFileExists = System.IO.File.Exists(viewPath),
                        ExpectedViewPath = $"Views/{controllerName}/{actionName}.cshtml"
                    };

                    ApplyTriage(item);
                    controllerVm.Actions.Add(item);
                }

                controllerVm.ActionsCount = controllerVm.Actions.Count;
                controllerVm.SmokeTargetsCount = controllerVm.Actions.Count(a => a.IsHttpGet);
                controllerVm.MissingViewsCount = controllerVm.Actions.Count(a => a.TriageCategory == "NeedsFix");
                model.Controllers.Add(controllerVm);
            }

            model.ControllersCount = model.Controllers.Count;
            model.ActionsCount = model.Controllers.Sum(x => x.ActionsCount);
            model.SmokeTargetsCount = model.Controllers.Sum(x => x.SmokeTargetsCount);
            model.MissingViewsCount = model.Controllers.Sum(x => x.MissingViewsCount);
            model.RequiresParametersCount = model.Controllers.SelectMany(x => x.Actions).Count(x => x.RequiresParameters);
            model.NeedsFixCount = model.Controllers.SelectMany(x => x.Actions).Count(x => x.TriageCategory == "NeedsFix");
            model.ManualCheckCount = model.Controllers.SelectMany(x => x.Actions).Count(x => x.TriageCategory == "ManualCheck");
            model.IgnoreCount = model.Controllers.SelectMany(x => x.Actions).Count(x => x.TriageCategory == "Ignore");
            model.ReadyCount = model.Controllers.SelectMany(x => x.Actions).Count(x => x.TriageCategory == "Ready");

            model.QuickNotes.Add("ابدأ أولًا بما هو Needs Fix لأنه أقرب شيء يحتاج تعديل حقيقي في الكود أو الـ Views.");
            model.QuickNotes.Add("Manual Check معناها جرّب الـ workflow من داخل الشاشة نفسها، خصوصًا Details/Edit/Review التي تحتاج بيانات موجودة.");
            model.QuickNotes.Add("Ignore لا تحتاج إصلاح View غالبًا، لأنها POST أو Download أو Export أو Action تشغيلية وليست صفحة عرض.");
            model.QuickNotes.Add("Ready معناها الشاشة قابلة للتجربة المباشرة من المتصفح الآن.");

            return model;
        }

        private static void ApplyTriage(QaActionSummaryVm item)
        {
            if (item.IsHttpPost)
            {
                item.StatusText = "POST action";
                item.TriageCategory = "Ignore";
                item.TriageReason = "هذه action من نوع POST ولا تحتاج View مستقلة عادة.";
                item.SuggestedNextStep = "اختبرها من الشاشة المرتبطة بها مثل Create أو Edit بعد فتح GET الخاصة بها.";
                return;
            }

            if (IsLikelyNonViewAction(item.ActionName))
            {
                item.StatusText = "Non-view action";
                item.TriageCategory = "Ignore";
                item.TriageReason = "هذه action غالبًا Download أو Export أو Ajax أو تشغيل داخلي وليست صفحة View.";
                item.SuggestedNextStep = "اختبرها من الزر المرتبط داخل الشاشة أو بعد تجهيز بيانات فعلية.";
                return;
            }

            if (item.IsHttpGet && !item.RequiresParameters && item.ViewFileExists)
            {
                item.StatusText = "Ready for smoke test";
                item.TriageCategory = "Ready";
                item.TriageReason = "GET بدون Parameters والـ View موجودة في المسار المتوقع.";
                item.SuggestedNextStep = "افتح الرابط مباشرة وجرّب الأزرار الأساسية داخل الصفحة.";
                return;
            }

            if (item.IsHttpGet && !item.RequiresParameters && !item.ViewFileExists)
            {
                item.StatusText = "View check needed";
                item.TriageCategory = "NeedsFix";
                item.TriageReason = "GET عادية بدون Parameters، وغالبًا يجب أن يكون لها View مباشرة لكنها غير موجودة في المسار المتوقع.";
                item.SuggestedNextStep = "راجع Controller ثم تأكد من وجود الملف في المسار المتوقع أو من أن return View يستخدم اسمًا مختلفًا.";
                return;
            }

            if (item.IsHttpGet && item.RequiresParameters)
            {
                item.StatusText = "Needs sample data";
                item.TriageCategory = "ManualCheck";
                item.TriageReason = "الصفحة تحتاج id أو query أو بيانات موجودة مسبقًا، لذلك لا يمكن الحكم عليها من اسم الـ View فقط.";
                item.SuggestedNextStep = "ادخل من Index أولًا ثم افتح السجل من الزر Details/Edit/Review بدل تجربة الرابط يدويًا.";
                return;
            }

            item.StatusText = "Manual review";
            item.TriageCategory = "ManualCheck";
            item.TriageReason = "الحالة غير قياسية وتحتاج مراجعة يدوية.";
            item.SuggestedNextStep = "راجع الـ controller والـ workflow المرتبط.";
        }

        private static bool IsMvcAction(MethodInfo method)
        {
            if (method.IsSpecialName) return false;
            if (method.GetCustomAttribute<NonActionAttribute>() is not null) return false;
            if (method.GetBaseDefinition().DeclaringType == typeof(Controller)) return false;

            var returnType = method.ReturnType;
            if (typeof(IActionResult).IsAssignableFrom(returnType)) return true;
            if (typeof(Task<IActionResult>).IsAssignableFrom(returnType)) return true;
            if (typeof(Task).IsAssignableFrom(returnType)) return true;
            return false;
        }

        private static bool IsInfrastructureParameter(Type type)
        {
            return type == typeof(string)
                   || type == typeof(int)
                   || type == typeof(int?)
                   || type == typeof(Guid)
                   || type == typeof(Guid?)
                   || type == typeof(DateTime)
                   || type == typeof(DateTime?)
                   || type == typeof(bool)
                   || type == typeof(bool?)
                   || type == typeof(decimal)
                   || type == typeof(decimal?)
                   || type == typeof(long)
                   || type == typeof(long?)
                   || type == typeof(double)
                   || type == typeof(double?);
        }

        private static string BuildSuggestedUrl(string controllerName, string actionName, bool requiresParameters)
        {
            if (string.Equals(actionName, "Index", StringComparison.OrdinalIgnoreCase))
            {
                return $"/{controllerName}";
            }

            if (requiresParameters)
            {
                return $"/{controllerName}/{actionName}/{{id-or-query}}";
            }

            return $"/{controllerName}/{actionName}";
        }

        private static bool IsLikelyNonViewAction(string actionName)
        {
            var names = new[]
            {
                "SeedNow", "Delete", "Download", "Export", "MarkRead", "MarkAllRead", "LoadMore", "ScanNow",
                "Upload", "Toggle", "Approve", "Reject", "ReturnForRevision", "ConvertToProject"
            };
            return names.Any(x => string.Equals(x, actionName, StringComparison.OrdinalIgnoreCase));
        }

        private static List<QaWorkflowScenarioVm> BuildWorkflows()
        {
            return new()
            {
                new QaWorkflowScenarioVm
                {
                    ModuleName = "المستفيدون",
                    ScenarioTitle = "ملف مستفيد كامل",
                    Steps = "Beneficiaries → Create → FamilyMembers → Documents → HumanitarianResearch → Review → Committee → AidRequest → AidCycle → Disbursement",
                    ExpectedResult = "الملف يتحرك بدون كسر بين الباحث والمراجع واللجنة ثم يظهر في الصرف والتقارير."
                },
                new QaWorkflowScenarioVm
                {
                    ModuleName = "التبرعات",
                    ScenarioTitle = "متبرع وتبرع وتخصيص",
                    Steps = "Donors → Create → Donations → DonationAllocations → Reports",
                    ExpectedResult = "التبرع يظهر في بيانات المتبرع ويؤثر في التقارير ويمكن تخصيصه لمستفيد أو مشروع."
                },
                new QaWorkflowScenarioVm
                {
                    ModuleName = "الكفالات",
                    ScenarioTitle = "كفيل وكفالة وصرف دوري",
                    Steps = "KafalaSponsors → KafalaCases → KafalaPayments → AidCycles",
                    ExpectedResult = "الكفالة ترتبط بالحالة وتدخل في دورة الصرف أو تظهر كمدفوعات مستقلة."
                },
                new QaWorkflowScenarioVm
                {
                    ModuleName = "المشروعات",
                    ScenarioTitle = "مقترح ثم مشروع",
                    Steps = "ProjectProposals → ConvertToProject → CharityProjects → ProjectPhases → ProjectActivities → ProjectPhaseTasks → ProjectTrackingLogs → ProjectAttachments",
                    ExpectedResult = "المشروع يتحول من مقترح إلى تنفيذ مع تتبع ومرفقات وتقارير."
                },
                new QaWorkflowScenarioVm
                {
                    ModuleName = "المحاضر والقرارات",
                    ScenarioTitle = "اجتماع ومحضر وقرار ومتابعة",
                    Steps = "BoardMeetings → Attendees → Minute → BoardDecisions → DecisionAttachments → FollowUps",
                    ExpectedResult = "المحضر يطبع والقرار يُتابع بمرفقاته دون مشاكل namespace أو views."
                },
                new QaWorkflowScenarioVm
                {
                    ModuleName = "الموارد البشرية",
                    ScenarioTitle = "ملف الموظف حتى المرتب",
                    Steps = "HrEmployees → Contracts → FundingAssignments → TaskAssignments → Attendance → PerformanceEvaluation → Bonuses → PayrollMonths",
                    ExpectedResult = "الموظف يظهر في HR والمرتبات ويؤثر في التقارير بلا أخطاء ربط."
                },
                new QaWorkflowScenarioVm
                {
                    ModuleName = "المحاسبة",
                    ScenarioTitle = "قيد وتقارير وربط تلقائي",
                    Steps = "FinancialAccount → CostCenters → FiscalPeriods → JournalEntries → AccountingReports → OperationalJournalEntries",
                    ExpectedResult = "القيود تترحل والتقارير تقرأ posted entries والحركات التشغيلية ترتبط محاسبيًا."
                }
            };
        }
    }
}
