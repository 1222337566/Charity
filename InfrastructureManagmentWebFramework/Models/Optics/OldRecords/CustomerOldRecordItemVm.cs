using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.OldRecords
{
    public class CustomerOldRecordItemVm
    {
        public Guid Id { get; set; }
        public DateTime RecordDateUtc { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Details { get; set; }
        public bool IsActive { get; set; }
    }
}
