using InfrastrfuctureManagmentCore.Domains.Prescriptions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Prescriptions;
using InfrastructureManagmentWebFramework.Models.Optics;
using Microsoft.AspNetCore.Mvc;
/// مؤجل مؤقتًا - خاص بمسار النظارات
/// غير مستخدم من الواجهة الحال
public class PrescriptionsController : Controller
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly ICustomerRepository _customerRepository;

    public PrescriptionsController(
        IPrescriptionRepository prescriptionRepository,
        ICustomerRepository customerRepository)
    {
        _prescriptionRepository = prescriptionRepository;
        _customerRepository = customerRepository;
    }

    public async Task<IActionResult> Index(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            return NotFound();

        var prescriptions = await _prescriptionRepository.GetByCustomerIdAsync(customerId);

        ViewBag.CustomerId = customer.Id;
        ViewBag.CustomerName = customer.NameAr;
        ViewBag.CustomerNumber = customer.CustomerNumber;

        var model = prescriptions.Select(x => new PrescriptionListItemVm
        {
            Id = x.Id,
            PrescriptionDateUtc = x.PrescriptionDateUtc,
            DoctorName = x.DoctorName,
            RightSph = x.RightSph,
            RightCyl = x.RightCyl,
            RightAxis = x.RightAxis,
            LeftSph = x.LeftSph,
            LeftCyl = x.LeftCyl,
            LeftAxis = x.LeftAxis,
            AddValue = x.AddValue,
            IPD = x.IPD,
            SHeight = x.SHeight,
            Remarks = x.Remarks,
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

        var vm = new CreatePrescriptionVm
        {
            CustomerId = customer.Id,
            CustomerNumber = customer.CustomerNumber,
            CustomerName = customer.NameAr,
            PrescriptionDateUtc = DateTime.UtcNow
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePrescriptionVm vm)
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

        var entity = new Prescription
        {
            Id = Guid.NewGuid(),
            CustomerId = vm.CustomerId,
            PrescriptionDateUtc = vm.PrescriptionDateUtc,
            DoctorName = vm.DoctorName?.Trim(),

            RightSph = vm.RightSph,
            RightCyl = vm.RightCyl,
            RightAxis = vm.RightAxis,

            LeftSph = vm.LeftSph,
            LeftCyl = vm.LeftCyl,
            LeftAxis = vm.LeftAxis,

            AddValue = vm.AddValue,
            IPD = vm.IPD,
            SHeight = vm.SHeight,
            Remarks = vm.Remarks?.Trim(),

            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _prescriptionRepository.AddAsync(entity);

        TempData["Success"] = "تم حفظ الروشتة بنجاح";
        return RedirectToAction(nameof(Index), new { customerId = vm.CustomerId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _prescriptionRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var customer = await _customerRepository.GetByIdAsync(entity.CustomerId);
        if (customer == null)
            return NotFound();

        var vm = new EditPrescriptionVm
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            CustomerNumber = customer.CustomerNumber,
            CustomerName = customer.NameAr,
            PrescriptionDateUtc = entity.PrescriptionDateUtc,
            DoctorName = entity.DoctorName,

            RightSph = entity.RightSph,
            RightCyl = entity.RightCyl,
            RightAxis = entity.RightAxis,

            LeftSph = entity.LeftSph,
            LeftCyl = entity.LeftCyl,
            LeftAxis = entity.LeftAxis,

            AddValue = entity.AddValue,
            IPD = entity.IPD,
            SHeight = entity.SHeight,
            Remarks = entity.Remarks,
            IsActive = entity.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditPrescriptionVm vm)
    {
        var entity = await _prescriptionRepository.GetByIdAsync(vm.Id);
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

        entity.PrescriptionDateUtc = vm.PrescriptionDateUtc;
        entity.DoctorName = vm.DoctorName?.Trim();

        entity.RightSph = vm.RightSph;
        entity.RightCyl = vm.RightCyl;
        entity.RightAxis = vm.RightAxis;

        entity.LeftSph = vm.LeftSph;
        entity.LeftCyl = vm.LeftCyl;
        entity.LeftAxis = vm.LeftAxis;

        entity.AddValue = vm.AddValue;
        entity.IPD = vm.IPD;
        entity.SHeight = vm.SHeight;
        entity.Remarks = vm.Remarks?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _prescriptionRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل الروشتة بنجاح";
        return RedirectToAction(nameof(Index), new { customerId = vm.CustomerId });
    }
}