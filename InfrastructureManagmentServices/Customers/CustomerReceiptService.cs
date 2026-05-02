using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Optics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Customers
{
    public class CustomerReceiptService : ICustomerReceiptService
    {
        private readonly AppDbContext _db;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ICustomerReceiptRepository _customerReceiptRepository;
        private readonly ICustomerAccountService _customerAccountService;

        public CustomerReceiptService(
            AppDbContext db,
            ICustomerRepository customerRepository,
            IPaymentMethodRepository paymentMethodRepository,
            ICustomerReceiptRepository customerReceiptRepository,
            ICustomerAccountService customerAccountService)
        {
            _db = db;
            _customerRepository = customerRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _customerReceiptRepository = customerReceiptRepository;
            _customerAccountService = customerAccountService;
        }

        public async Task<Guid> CreateAsync(CreateCustomerReceiptVm vm)
        {
            if (string.IsNullOrWhiteSpace(vm.ReceiptNumber))
                throw new InvalidOperationException("رقم السند مطلوب");

            if (vm.Amount <= 0)
                throw new InvalidOperationException("المبلغ يجب أن يكون أكبر من صفر");

            if (await _customerReceiptRepository.ReceiptNumberExistsAsync(vm.ReceiptNumber.Trim()))
                throw new InvalidOperationException("رقم السند موجود بالفعل");

            var customer = await _customerRepository.GetByIdAsync(vm.CustomerId);
            if (customer == null || !customer.IsActive)
                throw new InvalidOperationException("العميل غير موجود أو غير نشط");

            PaymentMethod? paymentMethod = null;
            if (vm.PaymentMethodId.HasValue && vm.PaymentMethodId.Value != Guid.Empty)
            {
                paymentMethod = await _paymentMethodRepository.GetByIdAsync(vm.PaymentMethodId.Value);
                if (paymentMethod == null || !paymentMethod.IsActive)
                    throw new InvalidOperationException("طريقة الدفع غير موجودة أو غير نشطة");
            }

            await using var trx = await _db.Database.BeginTransactionAsync();

            try
            {
                SalesInvoice? linkedInvoice = null;

                if (vm.SalesInvoiceId.HasValue && vm.SalesInvoiceId.Value != Guid.Empty)
                {
                    linkedInvoice = await _db.Set<SalesInvoice>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.Id == vm.SalesInvoiceId.Value &&
                            x.CustomerId == customer.Id);

                    if (linkedInvoice == null)
                        throw new InvalidOperationException("الفاتورة المرتبطة غير موجودة أو لا تخص هذا العميل");
                }
                var receipt = new CustomerReceipt
                {
                    Id = Guid.NewGuid(),
                    ReceiptNumber = vm.ReceiptNumber.Trim(),
                    ReceiptDateUtc = vm.ReceiptDateUtc,
                    CustomerId = customer.Id,
                    SalesInvoiceId = linkedInvoice?.Id,
                    PaymentMethodId = paymentMethod?.Id,
                    Amount = vm.Amount,
                    Notes = vm.Notes?.Trim(),
                    Status = CustomerReceiptStatus.Posted,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _customerReceiptRepository.AddAsync(receipt);
                await _customerAccountService.PostReceiptAsync(receipt);

                await trx.CommitAsync();
                return receipt.Id;
            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }
    }
}
