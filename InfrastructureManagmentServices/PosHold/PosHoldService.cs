using InfrastrfuctureManagmentCore.Domains.PosHolds;
using InfrastrfuctureManagmentCore.Persistence.Repositories.PosHolds;
using InfrastructureManagmentWebFramework.Models.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.PosHolds
{
    public class PosHoldService : IPosHoldService
    {
        private readonly IPosHoldRepository _posHoldRepository;

        public PosHoldService(IPosHoldRepository posHoldRepository)
        {
            _posHoldRepository = posHoldRepository;
        }

        public async Task HoldAsync(PosSaleVm vm)
        {
            if (vm.Lines == null || !vm.Lines.Any(x => x.ItemId != Guid.Empty && x.Quantity > 0))
                throw new InvalidOperationException("لا توجد أصناف لتعليقها");

            var hold = new PosHold
            {
                Id = Guid.NewGuid(),
                HoldNumber = GenerateHoldNumber(),
                HoldDateUtc = DateTime.UtcNow,
                CustomerName = string.IsNullOrWhiteSpace(vm.CustomerName) ? "عميل نقدي" : vm.CustomerName.Trim(),
                WarehouseId = vm.WarehouseId,
                PaymentMethodId = vm.PaymentMethodId == Guid.Empty ? null : vm.PaymentMethodId,
                Notes = vm.Notes?.Trim(),
                Status = PosHoldStatus.Held,
                CreatedAtUtc = DateTime.UtcNow
            };

            decimal subTotal = 0;
            decimal discount = 0;
            decimal tax = 0;

            foreach (var line in vm.Lines.Where(x => x.ItemId != Guid.Empty && x.Quantity > 0))
            {
                var lineTotal = (line.Quantity * line.UnitPrice) - line.DiscountAmount + line.TaxAmount;

                hold.Lines.Add(new PosHoldLine
                {
                    Id = Guid.NewGuid(),
                    ItemId = line.ItemId,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    DiscountAmount = line.DiscountAmount,
                    TaxAmount = line.TaxAmount,
                    LineTotal = lineTotal
                });

                subTotal += line.Quantity * line.UnitPrice;
                discount += line.DiscountAmount;
                tax += line.TaxAmount;
            }

            hold.SubTotal = subTotal;
            hold.DiscountAmount = discount;
            hold.TaxAmount = tax;
            hold.NetAmount = subTotal - discount + tax;

            await _posHoldRepository.AddAsync(hold);
        }

        public async Task<PosSaleVm?> ResumeAsync(Guid holdId)
        {
            var hold = await _posHoldRepository.GetByIdAsync(holdId);
            if (hold == null || hold.Status != PosHoldStatus.Held)
                return null;

            hold.Status = PosHoldStatus.Resumed;
            hold.UpdatedAtUtc = DateTime.UtcNow;
            await _posHoldRepository.UpdateAsync(hold);

            return new PosSaleVm
            {
                InvoiceNumber = "POS-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                InvoiceDateUtc = DateTime.UtcNow,
                CustomerName = hold.CustomerName,
                WarehouseId = hold.WarehouseId,
                PaymentMethodId = hold.PaymentMethodId ?? Guid.Empty,
                Notes = hold.Notes,
                Lines = hold.Lines.Select(x => new PosSaleLineVm
                {
                    ItemId = x.ItemId,
                    ItemCode = x.Item?.ItemCode ?? "",
                    ItemNameAr = x.Item?.ItemNameAr ?? "",
                    Barcode = x.Item?.Barcode,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    DiscountAmount = x.DiscountAmount,
                    TaxAmount = x.TaxAmount,
                    LineTotal = x.LineTotal
                }).ToList()
            };
        }

        public async Task MarkCompletedAsync(Guid holdId)
        {
            var hold = await _posHoldRepository.GetByIdAsync(holdId);
            if (hold == null)
                return;

            hold.Status = PosHoldStatus.Completed;
            hold.UpdatedAtUtc = DateTime.UtcNow;
            await _posHoldRepository.UpdateAsync(hold);
        }

        private static string GenerateHoldNumber()
        {
            return "HOLD-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
