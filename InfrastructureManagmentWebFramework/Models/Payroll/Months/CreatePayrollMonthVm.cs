using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Payroll.Months
{
    public class CreatePayrollMonthVm
    {
        [Range(2020, 2100), Display(Name = "السنة")]
        public int Year { get; set; } = DateTime.Today.Year;

        [Range(1, 12), Display(Name = "الشهر")]
        public int Month { get; set; } = DateTime.Today.Month;
    }
}
