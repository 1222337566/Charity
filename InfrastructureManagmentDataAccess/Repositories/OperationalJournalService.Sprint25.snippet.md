## تحديث مقترح على `OperationalJournalService`

### داخل `CreateExpenseEntryAsync`
بعد الحصول على `ProjectExpenseLink` أضف فحصًا لربط المرحلة:

```csharp
var phaseLink = await _db.Set<ProjectPhaseExpenseLink>()
    .AsNoTracking()
    .FirstOrDefaultAsync(x => x.ExpenseId == expenseId);

if (phaseLink != null)
{
    projectId = phaseLink.ProjectId;
    costCenterId = phaseLink.CostCenterId;

    if (!costCenterId.HasValue)
    {
        var phase = await _db.Set<ProjectPhase>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == phaseLink.ProjectPhaseId);
        costCenterId = phase?.CostCenterId;
    }
}
```

### داخل `CreateStoreIssueEntryAsync`
قبل إنشاء القيد أضف:

```csharp
var phaseLink = await _db.Set<ProjectPhaseStoreIssueLink>()
    .AsNoTracking()
    .FirstOrDefaultAsync(x => x.StoreIssueId == storeIssueId);

if (phaseLink != null)
{
    if (phaseLink.ProjectId != Guid.Empty)
        issue.ProjectId = phaseLink.ProjectId;

    if (phaseLink.CostCenterId.HasValue)
        costCenterId = phaseLink.CostCenterId;
    else
    {
        var phase = await _db.Set<ProjectPhase>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == phaseLink.ProjectPhaseId);
        costCenterId = phase?.CostCenterId ?? costCenterId;
    }
}
```
