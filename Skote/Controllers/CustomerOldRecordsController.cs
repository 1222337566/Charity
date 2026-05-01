using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastructureManagmentWebFramework.Models.Optics;
using Microsoft.AspNetCore.Mvc;
/// مؤجل مؤقتًا - خاص بمسار النظارات
/// غير مستخدم من الواجهة الحال
public class CustomerOldRecordsController : Controller
{
    private readonly ICustomerOldRecordRepository _customerOldRecordRepository;
    private readonly ICustomerRepository _customerRepository;

    public CustomerOldRecordsController(
        ICustomerOldRecordRepository customerOldRecordRepository,
        ICustomerRepository customerRepository)
    {
        _customerOldRecordRepository = customerOldRecordRepository;
        _customerRepository = customerRepository;
    }

    public async Task<IActionResult> Index(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            return NotFound();

        var records = await _customerOldRecordRepository.GetByCustomerIdAsync(customerId);

        ViewBag.CustomerId = customer.Id;
        ViewBag.CustomerName = customer.NameAr;
        ViewBag.CustomerNumber = customer.CustomerNumber;

        var model = records.Select(x => new OldRecordListItemVm
        {
            Id = x.Id,
            RecordDateUtc = x.RecordDateUtc,
            Title = x.Title,
            Details = x.Details,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            return NotFound();

        var vm = new CreateOldRecordVm
        {
            CustomerId = customer.Id,
            CustomerNumber = customer.CustomerNumber,
            CustomerName = customer.NameAr,
            RecordDateUtc = DateTime.UtcNow
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOldRecordVm vm)
    {
        var customer = await _customerRepository.GetByIdAsync(vm.CustomerId);
        if (customer == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            vm.CustomerNumber = customer.CustomerNumber;
            vm.CustomerName = customer.NameAr;
            return View(vm);
        }

        var entity = new CustomerOldRecord
        {
            Id = Guid.NewGuid(),
            CustomerId = vm.CustomerId,
            RecordDateUtc = vm.RecordDateUtc,
            Title = vm.Title.Trim(),
            Details = vm.Details?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _customerOldRecordRepository.AddAsync(entity);

        TempData["Success"] = "تم حفظ السجل القديم بنجاح";
        return RedirectToAction(nameof(Index), new { customerId = vm.CustomerId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _customerOldRecordRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var customer = await _customerRepository.GetByIdAsync(entity.CustomerId);
        if (customer == null)
            return NotFound();

        var vm = new EditOldRecordVm
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            CustomerNumber = customer.CustomerNumber,
            CustomerName = customer.NameAr,
            RecordDateUtc = entity.RecordDateUtc,
            Title = entity.Title,
            Details = entity.Details,
            IsActive = entity.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditOldRecordVm vm)
    {
        var entity = await _customerOldRecordRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        var customer = await _customerRepository.GetByIdAsync(vm.CustomerId);
        if (customer == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            vm.CustomerNumber = customer.CustomerNumber;
            vm.CustomerName = customer.NameAr;
            return View(vm);
        }

        entity.RecordDateUtc = vm.RecordDateUtc;
        entity.Title = vm.Title.Trim();
        entity.Details = vm.Details?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _customerOldRecordRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل السجل القديم بنجاح";
        return RedirectToAction(nameof(Index), new { customerId = vm.CustomerId });
    }
}