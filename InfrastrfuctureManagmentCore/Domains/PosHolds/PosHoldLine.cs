using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.PosHolds
{
    public class PosHoldLine
    {
        public Guid Id { get; set; }

        public Guid PosHoldId { get; set; }
        public PosHold? PosHold { get; set; }

        public Guid ItemId { get; set; }
        public InfrastrfuctureManagmentCore.Domains.Item.Item? Item { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
}
