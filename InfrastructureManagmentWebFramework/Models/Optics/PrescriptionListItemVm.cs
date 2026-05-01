using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    public class PrescriptionListItemVm
    {
        public Guid Id { get; set; }
        public DateTime PrescriptionDateUtc { get; set; }
        public string? DoctorName { get; set; }

        public decimal? RightSph { get; set; }
        public decimal? RightCyl { get; set; }
        public decimal? RightAxis { get; set; }

        public decimal? LeftSph { get; set; }
        public decimal? LeftCyl { get; set; }
        public decimal? LeftAxis { get; set; }

        public decimal? AddValue { get; set; }
        public decimal? IPD { get; set; }
        public decimal? SHeight { get; set; }

        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}
