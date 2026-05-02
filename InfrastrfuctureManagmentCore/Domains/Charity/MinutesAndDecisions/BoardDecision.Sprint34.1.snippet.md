حدّث `BoardDecision.cs` بإضافة collection للمرفقات:

```csharp
public ICollection<BoardDecisionAttachment> Attachments { get; set; } = new List<BoardDecisionAttachment>();
```
