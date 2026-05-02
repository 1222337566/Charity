using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.HR.Advanced.Evaluations
{
    public class CreatePerformanceEvaluationVm
    {
        public Guid Id { get; set; }
        [Required, Display(Name = "الموظف")]
        public Guid EmployeeId { get; set; }
        [Display(Name = "المقيم")]
        public Guid? EvaluatorEmployeeId { get; set; }
        [Required, Display(Name = "فترة التقييم")]
        public string EvaluationPeriod { get; set; } = string.Empty;
        [DataType(DataType.Date), Display(Name = "تاريخ التقييم")]
        public DateTime EvaluationDate { get; set; } = DateTime.Today;
        [Range(0,100), Display(Name = "الانضباط")]
        public decimal DisciplineScore { get; set; }
        [Range(0,100), Display(Name = "الأداء")]
        public decimal PerformanceScore { get; set; }
        [Range(0,100), Display(Name = "التعاون")]
        public decimal CooperationScore { get; set; }
        [Range(0,100), Display(Name = "المبادرة")]
        public decimal InitiativeScore { get; set; }
        [Display(Name = "النتيجة")]
        public string Result { get; set; } = "Good";
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        public List<SelectListItem> Employees { get; set; } = new();
        public List<SelectListItem> Results { get; set; } = new();
    }
}
