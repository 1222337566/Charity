namespace Skote.Helpers;

public static class UserActivityBusinessActions
{
    public const string ResearchSubmittedForReview = "إرسال استمارة للمراجعة";
    public const string ResearchReviewed = "مراجعة الاستمارة";
    public const string ResearchSentToCommittee = "إحالة الاستمارة للجنة";
    public const string ResearchCommitteeDecision = "قرار اللجنة على الاستمارة";

    public const string CommitteeDecisionCreated = "إنشاء قرار لجنة";
    public const string CommitteeDecisionUpdated = "تعديل قرار لجنة";

    public const string AidCycleCreated = "إنشاء دورة صرف";
    public const string AidCycleUpdated = "تعديل دورة صرف";
    public const string AidCycleGenerated = "توليد دورة صرف";
    public const string AidCycleApproved = "اعتماد دورة صرف";
    public const string AidCycleClosed = "إغلاق دورة صرف";
    public const string AidCycleBatchDisbursed = "الصرف الجماعي من دورة صرف";

    public const string BeneficiaryDisbursementCreated = "إنشاء صرف مستفيد";
}
