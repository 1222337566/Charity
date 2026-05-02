using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Models.Funding
{
    public class GrantorIndexVm
    {
        public List<GrantorRowVm> Rows { get; set; } = new();
    }

    public class GrantorRowVm
    {
        public Guid Id { get; set; }
        public string GrantorCode { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalFunding { get; set; }
        public int AgreementsCount { get; set; }
    }

    public class CreateGrantorVm
    {
        [Required]
        [Display(Name = "كود الجهة")]
        public string GrantorCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم الجهة المانحة")]
        public string NameAr { get; set; } = string.Empty;

        [Display(Name = "اسم الجهة بالإنجليزية")]
        public string? NameEn { get; set; }

        [Display(Name = "مسئول التواصل")]
        public string? ContactPerson { get; set; }

        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string? Email { get; set; }

        [Display(Name = "التليفون")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }

    public class ProjectFundingAgreementIndexVm
    {
        public Guid? ProjectId { get; set; }
        public Guid? GrantorId { get; set; }
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> Grantors { get; set; } = new();
        public List<ProjectFundingAgreementRowVm> Rows { get; set; } = new();

        public decimal TotalFunding => Rows.Sum(x => x.FundingAmount);
        public decimal TotalReceived => Rows.Sum(x => x.ReceivedAmount);
        public decimal TotalRemaining => TotalFunding - TotalReceived;
    }

    public class ProjectFundingAgreementRowVm
    {
        public Guid Id { get; set; }
        public string AgreementNumber { get; set; } = string.Empty;
        public string GrantorName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public decimal FundingAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal RemainingAmount => FundingAmount - ReceivedAmount;
        public DateTime StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? ContactEmail { get; set; }
    }

    public class CreateProjectFundingAgreementVm
    {
        [Required]
        [Display(Name = "رقم الاتفاقية")]
        public string AgreementNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "الجهة المانحة")]
        public Guid GrantorId { get; set; }

        [Required]
        [Display(Name = "المشروع")]
        public Guid ProjectId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "قيمة التمويل")]
        public decimal FundingAmount { get; set; }

        [Required]
        [Display(Name = "تاريخ بدء المشروع/الاتفاقية")]
        public DateTime StartDateUtc { get; set; } = DateTime.UtcNow;

        [Display(Name = "تاريخ نهاية الاتفاقية")]
        public DateTime? EndDateUtc { get; set; }

        [Display(Name = "مسئول التواصل")]
        public string? ContactPerson { get; set; }

        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string? ContactEmail { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Grantors { get; set; } = new();
        public List<SelectListItem> Projects { get; set; } = new();
    }

    public class ProjectFundingReportVm
    {
        public Guid? ProjectId { get; set; }
        public Guid? GrantorId { get; set; }
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> Grantors { get; set; } = new();
        public List<ProjectFundingAgreementRowVm> Rows { get; set; } = new();
        public decimal TotalFunding => Rows.Sum(x => x.FundingAmount);
        public decimal TotalReceived => Rows.Sum(x => x.ReceivedAmount);
        public decimal TotalRemaining => TotalFunding - TotalReceived;
    }
}
