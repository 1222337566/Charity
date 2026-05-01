using InfrastrfuctureManagmentCore.Domains.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastructureManagmentWebFramework.Models.Optics.WorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Optics
{
    public class OpticalWorkOrderService : IOpticalWorkOrderService
    {
        private readonly ISalesInvoiceRepository _salesInvoiceRepository;
        private readonly IOpticalWorkOrderRepository _opticalWorkOrderRepository;

        public OpticalWorkOrderService(
            ISalesInvoiceRepository salesInvoiceRepository,
            IOpticalWorkOrderRepository opticalWorkOrderRepository)
        {
            _salesInvoiceRepository = salesInvoiceRepository;
            _opticalWorkOrderRepository = opticalWorkOrderRepository;
        }

        public async Task<Guid> CreateAsync(CreateOpticalWorkOrderVm vm)
        {
            if (string.IsNullOrWhiteSpace(vm.WorkOrderNumber))
                throw new InvalidOperationException("رقم أمر الشغل مطلوب");

            if (await _opticalWorkOrderRepository.WorkOrderNumberExistsAsync(vm.WorkOrderNumber.Trim()))
                throw new InvalidOperationException("رقم أمر الشغل موجود بالفعل");

            var existing = await _opticalWorkOrderRepository.GetBySalesInvoiceIdAsync(vm.SalesInvoiceId);
            if (existing != null)
                throw new InvalidOperationException("تم إنشاء أمر شغل لهذه الفاتورة بالفعل");

            var invoice = await _salesInvoiceRepository.GetByIdAsync(vm.SalesInvoiceId);
            if (invoice == null)
                throw new InvalidOperationException("فاتورة البيع غير موجودة");

            var entity = new OpticalWorkOrder
            {
                Id = Guid.NewGuid(),
                WorkOrderNumber = vm.WorkOrderNumber.Trim(),
                SalesInvoiceId = invoice.Id,
                CustomerId = invoice.CustomerId,
                PrescriptionId = invoice.PrescriptionId,
                OrderDateUtc = vm.OrderDateUtc,
                ExpectedDeliveryDateUtc = vm.ExpectedDeliveryDateUtc,
                Status = OpticalWorkOrderStatus.New,
                IsUrgent = vm.IsUrgent,
                FrameNotes = vm.FrameNotes?.Trim(),
                LensNotes = vm.LensNotes?.Trim(),
                WorkshopNotes = vm.WorkshopNotes?.Trim(),
                DeliveryNotes = vm.DeliveryNotes?.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            };

            await _opticalWorkOrderRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task MarkInLabAsync(Guid id)
        {
            var entity = await _opticalWorkOrderRepository.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException("أمر الشغل غير موجود");

            entity.Status = OpticalWorkOrderStatus.InLab;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _opticalWorkOrderRepository.UpdateAsync(entity);
        }

        public async Task MarkReadyAsync(Guid id)
        {
            var entity = await _opticalWorkOrderRepository.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException("أمر الشغل غير موجود");

            entity.Status = OpticalWorkOrderStatus.Ready;
            entity.ReadyDateUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _opticalWorkOrderRepository.UpdateAsync(entity);
        }

        public async Task MarkDeliveredAsync(Guid id)
        {
            var entity = await _opticalWorkOrderRepository.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException("أمر الشغل غير موجود");

            entity.Status = OpticalWorkOrderStatus.Delivered;
            entity.DeliveredDateUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _opticalWorkOrderRepository.UpdateAsync(entity);
        }
    }
}
