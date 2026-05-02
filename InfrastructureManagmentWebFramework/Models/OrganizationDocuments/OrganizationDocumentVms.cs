using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Models.OrganizationDocuments
{
    public class OrganizationDocumentIndexVm
    {
        public string? DocumentType { get; set; }

        public string? SearchText { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public bool IncludeArchived { get; set; }

        public List<SelectListItem> DocumentTypes { get; set; } = new();

        public List<OrganizationDocumentRowVm> Rows { get; set; } = new();

        public int TotalCount => Rows.Count;

        public long TotalSize => Rows.Sum(x => x.FileSize);
    }

    public class OrganizationDocumentRowVm
    {
        public Guid Id { get; set; }

        public string DocumentNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string DocumentType { get; set; } = string.Empty;

        public string DocumentTypeAr { get; set; } = string.Empty;

        public DateTime DocumentDateUtc { get; set; }

        public string? RelatedEntityType { get; set; }

        public string? RelatedEntityName { get; set; }

        public string FileName { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public bool IsArchived { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string? Notes { get; set; }
    }

    public class CreateOrganizationDocumentVm
    {
        [Required]
        [Display(Name = "رقم المستند")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "عنوان المستند")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "نوع المستند")]
        public string DocumentType { get; set; } = "Other";

        [Required]
        [Display(Name = "تاريخ المستند")]
        public DateTime DocumentDateUtc { get; set; } = DateTime.UtcNow;

        [Display(Name = "نوع الكيان المرتبط")]
        public string? RelatedEntityType { get; set; }

        [Display(Name = "معرف الكيان المرتبط")]
        public Guid? RelatedEntityId { get; set; }

        [Display(Name = "اسم الكيان المرتبط")]
        public string? RelatedEntityName { get; set; }

        [Required]
        [Display(Name = "ملف المستند")]
        public IFormFile? File { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> DocumentTypes { get; set; } = new();

        public List<SelectListItem> RelatedEntityTypes { get; set; } = new();
    }

    public static class OrganizationDocumentTypeOptions
    {
        public static List<SelectListItem> GetDocumentTypes(string? selected = null)
        {
            var items = new Dictionary<string, string>
            {
                ["EmployeeContract"] = "عقد موظف",
                ["OfficialLetter"] = "خطاب رسمي",
                ["BoardMeetingMinutes"] = "محضر مجلس إدارة",
                ["CooperationAgreement"] = "اتفاقية تعاون",
                ["ProjectContract"] = "عقد مشروع",
                ["AssociationPaper"] = "أوراق خاصة بالجمعية",
                ["GrantorDocument"] = "مستند جهة مانحة",
                ["ProjectDocument"] = "مستند مشروع",
                ["BeneficiaryDocument"] = "مستند مستفيد",
                ["Other"] = "أخرى"
            };

            return items.Select(x => new SelectListItem
            {
                Value = x.Key,
                Text = x.Value,
                Selected = selected == x.Key
            }).ToList();
        }

        public static List<SelectListItem> GetRelatedEntityTypes(string? selected = null)
        {
            var items = new Dictionary<string, string>
            {
                ["Association"] = "الجمعية",
                ["Employee"] = "موظف",
                ["Project"] = "مشروع",
                ["Grantor"] = "جهة مانحة",
                ["Beneficiary"] = "مستفيد",
                ["Board"] = "مجلس الإدارة",
                ["Government"] = "جهة حكومية",
                ["Other"] = "أخرى"
            };

            return items.Select(x => new SelectListItem
            {
                Value = x.Key,
                Text = x.Value,
                Selected = selected == x.Key
            }).ToList();
        }

        public static string ToArabic(string? documentType)
        {
            return GetDocumentTypes(documentType)
                .FirstOrDefault(x => x.Value == documentType || x.Selected)?.Text
                ?? documentType
                ?? "أخرى";
        }
    }
}
