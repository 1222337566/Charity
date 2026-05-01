using System;
using System.Collections.Generic;

namespace Skote.ViewModels.QaAudit
{
    public class QaAuditIndexVm
    {
        public DateTime GeneratedAtUtc { get; set; }
        public int ControllersCount { get; set; }
        public int ActionsCount { get; set; }
        public int SmokeTargetsCount { get; set; }
        public int MissingViewsCount { get; set; }
        public int RequiresParametersCount { get; set; }
        public int NeedsFixCount { get; set; }
        public int ManualCheckCount { get; set; }
        public int IgnoreCount { get; set; }
        public int ReadyCount { get; set; }
        public List<QaControllerSummaryVm> Controllers { get; set; } = new();
        public List<QaWorkflowScenarioVm> Workflows { get; set; } = new();
        public List<string> QuickNotes { get; set; } = new();
    }

    public class QaControllerSummaryVm
    {
        public string ControllerName { get; set; } = string.Empty;
        public int ActionsCount { get; set; }
        public int SmokeTargetsCount { get; set; }
        public int MissingViewsCount { get; set; }
        public List<QaActionSummaryVm> Actions { get; set; } = new();
    }

    public class QaActionSummaryVm
    {
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string SuggestedUrl { get; set; } = string.Empty;
        public bool IsHttpGet { get; set; }
        public bool IsHttpPost { get; set; }
        public bool RequiresParameters { get; set; }
        public string ParametersDescription { get; set; } = string.Empty;
        public bool ViewFileExists { get; set; }
        public string ExpectedViewPath { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public string TriageCategory { get; set; } = string.Empty; // NeedsFix / ManualCheck / Ignore / Ready
        public string TriageReason { get; set; } = string.Empty;
        public string SuggestedNextStep { get; set; } = string.Empty;
    }

    public class QaWorkflowScenarioVm
    {
        public string ModuleName { get; set; } = string.Empty;
        public string ScenarioTitle { get; set; } = string.Empty;
        public string Steps { get; set; } = string.Empty;
        public string ExpectedResult { get; set; } = string.Empty;
    }
}
