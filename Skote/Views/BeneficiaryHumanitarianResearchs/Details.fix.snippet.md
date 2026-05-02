## Fix for `ReviewDate` compile error in Details.cshtml

If your entity `BeneficiaryHumanitarianResearchReview` does not have `ReviewDate`, use the actual property name from your entity.

Common replacements:

- `CreatedAtUtc`
- `ReviewedAtUtc`
- `ReviewAtUtc`

Safe fallback example:

```cshtml
@* replace review.ReviewDate *@
@(review.CreatedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm"))
```
