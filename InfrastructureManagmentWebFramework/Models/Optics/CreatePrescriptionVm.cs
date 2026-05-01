using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    
    public class CreatePrescriptionVm
    {
        public Guid CustomerId { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "التاريخ")]
        public DateTime PrescriptionDateUtc { get; set; } = DateTime.UtcNow;

        [Display(Name = "اسم الدكتور")]
        public string? DoctorName { get; set; }

        [Display(Name = "Right Sph")]
        public decimal? RightSph { get; set; }

        [Display(Name = "Right Cyl")]
        public decimal? RightCyl { get; set; }

        [Display(Name = "Right Axis")]
        public decimal? RightAxis { get; set; }

        [Display(Name = "Left Sph")]
        public decimal? LeftSph { get; set; }

        [Display(Name = "Left Cyl")]
        public decimal? LeftCyl { get; set; }

        [Display(Name = "Left Axis")]
        public decimal? LeftAxis { get; set; }

        [Display(Name = "Add")]
        public decimal? AddValue { get; set; }

        [Display(Name = "IPD")]
        public decimal? IPD { get; set; }

        [Display(Name = "S Height")]
        public decimal? SHeight { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Remarks { get; set; }
    }
}
