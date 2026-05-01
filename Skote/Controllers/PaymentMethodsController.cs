using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastructureManagmentWebFramework.Models.PaymentMethods;
using Microsoft.AspNetCore.Mvc;

public class PaymentMethodsController : Controller
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public PaymentMethodsController(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<IActionResult> Index()
    {
        var methods = await _paymentMethodRepository.GetAllAsync();

        var model = methods.Select(x => new PaymentMethodListItemVm
        {
            Id = x.Id,
            MethodCode = x.MethodCode,
            MethodNameAr = x.MethodNameAr,
            MethodNameEn = x.MethodNameEn,
            IsCash = x.IsCash,
            IsDefault = x.IsDefault,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePaymentMethodVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePaymentMethodVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (await _paymentMethodRepository.CodeExistsAsync(vm.MethodCode.Trim()))
        {
            ModelState.AddModelError(nameof(vm.MethodCode), "كود الطريقة موجود بالفعل");
            return View(vm);
        }

        var entity = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            MethodCode = vm.MethodCode.Trim(),
            MethodNameAr = vm.MethodNameAr.Trim(),
            MethodNameEn = vm.MethodNameEn?.Trim(),
            IsCash = vm.IsCash,
            IsDefault = vm.IsDefault,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _paymentMethodRepository.AddAsync(entity);

        TempData["Success"] = "تم إضافة طريقة الدفع بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _paymentMethodRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditPaymentMethodVm
        {
            Id = entity.Id,
            MethodCode = entity.MethodCode,
            MethodNameAr = entity.MethodNameAr,
            MethodNameEn = entity.MethodNameEn,
            IsCash = entity.IsCash,
            IsDefault = entity.IsDefault,
            IsActive = entity.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditPaymentMethodVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _paymentMethodRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (await _paymentMethodRepository.CodeExistsAsync(vm.MethodCode.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.MethodCode), "كود الطريقة موجود بالفعل");
            return View(vm);
        }

        entity.MethodCode = vm.MethodCode.Trim();
        entity.MethodNameAr = vm.MethodNameAr.Trim();
        entity.MethodNameEn = vm.MethodNameEn?.Trim();
        entity.IsCash = vm.IsCash;
        entity.IsDefault = vm.IsDefault;
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _paymentMethodRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل طريقة الدفع بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var entity = await _paymentMethodRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        entity.IsActive = !entity.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _paymentMethodRepository.UpdateAsync(entity);

        TempData["Success"] = entity.IsActive
            ? "تم تفعيل طريقة الدفع"
            : "تم تعطيل طريقة الدفع";

        return RedirectToAction(nameof(Index));
    }
}