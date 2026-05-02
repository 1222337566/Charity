namespace InfrastrfuctureManagmentCore.Domains.Charity.Workspace
{
    /// <summary>
    /// تخطيط مساحة العمل المحفوظ لكل مستخدم
    /// يُخزَّن كـ JSON: قائمة من { WidgetKey, Order, Visible, ColSpan }
    /// </summary>
    public class UserWorkspaceLayout
    {
        public Guid   Id           { get; set; }
        public string UserId       { get; set; } = null!;
        public string LayoutJson   { get; set; } = "[]";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
