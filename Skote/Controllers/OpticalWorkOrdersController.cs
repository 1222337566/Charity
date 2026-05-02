using InfrastrfuctureManagmentCore.Domains.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastructureManagmentServices.Optics;
using InfrastructureManagmentWebFramework.Models.Optics.WorkOrders;
using Microsoft.AspNetCore.Mvc;
/// مؤجل مؤقتًا - خاص بمسار النظارات
/// غير مستخدم من الواجهة الحال
public class OpticalWorkOrdersController : Controller
{
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly IOpticalWorkOrderRepository _opticalWorkOrderRepository;
    private readonly IOpticalWorkOrderService _opticalWorkOrderService;

    public OpticalWorkOrdersController(
        ISalesInvoiceRepository salesInvoiceRepository,
        IOpticalWorkOrderRepository opticalWorkOrderRepository,
        IOpticalWorkOrderService opticalWorkOrderService)
    {
        _salesInvoiceRepository = salesInvoiceRepository;
        _opticalWorkOrderRepository = opticalWorkOrderRepository;
        _opticalWorkOrderService = opticalWorkOrderService;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _opticalWorkOrderRepository.GetAllAsync();

        var model = orders.Select(x => new OpticalWorkOrderListItemVm
        {
            Id = x.Id,
            WorkOrderNumber = x.WorkOrderNumber,
            InvoiceNumber = x.SalesInvoice?.InvoiceNumber ?? "-",
            CustomerName = x.Customer?.NameAr ?? x.SalesInvoice?.CustomerName ?? "-",
            OrderDateUtc = x.OrderDateUtc,
            ExpectedDeliveryDateUtc = x.ExpectedDeliveryDateUtc,
            IsUrgent = x.IsUrgent,
            StatusText = x.Status.ToString()
        }).ToList();

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Print(Guid id)
    {
        var entity = await _opticalWorkOrderRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new OpticalWorkOrderPrintVm
        {
            WorkOrderNumber = entity.WorkOrderNumber,
            InvoiceNumber = entity.SalesInvoice?.InvoiceNumber ?? "-",
            CustomerName = entity.Customer?.NameAr ?? entity.SalesInvoice?.CustomerName ?? "-",
            CustomerNumber = entity.Customer?.CustomerNumber,
            PrescriptionText = entity.Prescription != null
                ? $"{entity.Prescription.PrescriptionDateUtc.ToLocalTime():yyyy-MM-dd} - Dr: {(string.IsNullOrWhiteSpace(entity.Prescription.DoctorName) ? "-" : entity.Prescription.DoctorName)}"
                : null,
            OrderDateUtc = entity.OrderDateUtc,
            ExpectedDeliveryDateUtc = entity.ExpectedDeliveryDateUtc,
            StatusText = entity.Status switch
            {
                OpticalWorkOrderStatus.New => "جديد",
                OpticalWorkOrderStatus.InLab => "في المعمل",
                OpticalWorkOrderStatus.Ready => "جاهز",
                OpticalWorkOrderStatus.Delivered => "تم التسليم",
                OpticalWorkOrderStatus.Cancelled => "ملغي",
                _ => "-"
            },
            IsUrgent = entity.IsUrgent,
            FrameNotes = entity.FrameNotes,
            LensNotes = entity.LensNotes,
            WorkshopNotes = entity.WorkshopNotes,
            DeliveryNotes = entity.DeliveryNotes,
            Items = entity.SalesInvoice?.Lines.Select(x => new OpticalWorkOrderLineVm
            {
                ItemCode = x.Item?.ItemCode ?? "",
                ItemNameAr = x.Item?.ItemNameAr ?? "",
                Quantity = x.Quantity
            }).ToList() ?? new List<OpticalWorkOrderLineVm>()
        };

        return View(vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeliverFromReadyQueue(Guid id)
    {
        await _opticalWorkOrderService.MarkDeliveredAsync(id);
        TempData["Success"] = "تم تسليم الطلب بنجاح";
        return RedirectToAction(nameof(ReadyForDelivery));
    }
    [HttpGet]
    public async Task<IActionResult> ReadyForDelivery()
    {
        var orders = await _opticalWorkOrderRepository.GetReadyForDeliveryAsync();

        var model = orders.Select(x => new ReadyForDeliveryItemVm
        {
            Id = x.Id,
            WorkOrderNumber = x.WorkOrderNumber,
            InvoiceNumber = x.SalesInvoice?.InvoiceNumber ?? "-",
            CustomerName = x.Customer?.NameAr ?? x.SalesInvoice?.CustomerName ?? "-",
            CustomerNumber = x.Customer?.CustomerNumber,
            CustomerMobile = x.Customer?.MobileNo,
            PrescriptionText = x.Prescription != null
                ? x.Prescription.PrescriptionDateUtc.ToLocalTime().ToString("yyyy-MM-dd")
                : null,
            OrderDateUtc = x.OrderDateUtc,
            ExpectedDeliveryDateUtc = x.ExpectedDeliveryDateUtc,
            ReadyDateUtc = x.ReadyDateUtc,
            IsUrgent = x.IsUrgent,
            IsLateForDelivery = x.ExpectedDeliveryDateUtc.HasValue && x.ExpectedDeliveryDateUtc.Value < DateTime.UtcNow,
            DeliveryNotes = x.DeliveryNotes
        }).ToList();

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Dashboard(string? filter = null)
    {
        var all = await _opticalWorkOrderRepository.GetAllAsync();

        var today = DateTime.UtcNow.Date;

        var openOrders = all
            .Where(x => x.Status != OpticalWorkOrderStatus.Delivered && x.Status != OpticalWorkOrderStatus.Cancelled)
            .ToList();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            filter = filter.Trim().ToLowerInvariant();

            openOrders = filter switch
            {
                "new" => openOrders.Where(x => x.Status == OpticalWorkOrderStatus.New).ToList(),
                "inlab" => openOrders.Where(x => x.Status == OpticalWorkOrderStatus.InLab).ToList(),
                "ready" => openOrders.Where(x => x.Status == OpticalWorkOrderStatus.Ready).ToList(),
                "overdue" => openOrders.Where(x =>
                    x.ExpectedDeliveryDateUtc.HasValue &&
                    x.ExpectedDeliveryDateUtc.Value < DateTime.UtcNow).ToList(),
                _ => openOrders
            };
        }

        var vm = new WorkOrdersDashboardVm
        {
            NewCount = all.Count(x => x.Status == OpticalWorkOrderStatus.New),
            InLabCount = all.Count(x => x.Status == OpticalWorkOrderStatus.InLab),
            ReadyCount = all.Count(x => x.Status == OpticalWorkOrderStatus.Ready),
            DeliveredTodayCount = all.Count(x =>
                x.Status == OpticalWorkOrderStatus.Delivered &&
                x.DeliveredDateUtc.HasValue &&
                x.DeliveredDateUtc.Value.Date == today),
            OverdueCount = all.Count(x =>
                x.Status != OpticalWorkOrderStatus.Delivered &&
                x.Status != OpticalWorkOrderStatus.Cancelled &&
                x.ExpectedDeliveryDateUtc.HasValue &&
                x.ExpectedDeliveryDateUtc.Value < DateTime.UtcNow),

            Orders = openOrders
                .OrderBy(x => x.ExpectedDeliveryDateUtc ?? DateTime.MaxValue)
                .ThenByDescending(x => x.IsUrgent)
                .Select(x => new OpticalWorkOrderDashboardItemVm
                {
                    Id = x.Id,
                    WorkOrderNumber = x.WorkOrderNumber,
                    InvoiceNumber = x.SalesInvoice?.InvoiceNumber ?? "-",
                    CustomerName = x.Customer?.NameAr ?? x.SalesInvoice?.CustomerName ?? "-",
                    PrescriptionText = x.Prescription != null
                        ? x.Prescription.PrescriptionDateUtc.ToLocalTime().ToString("yyyy-MM-dd")
                        : null,
                    OrderDateUtc = x.OrderDateUtc,
                    ExpectedDeliveryDateUtc = x.ExpectedDeliveryDateUtc,
                    StatusText = x.Status switch
                    {
                        OpticalWorkOrderStatus.New => "جديد",
                        OpticalWorkOrderStatus.InLab => "في المعمل",
                        OpticalWorkOrderStatus.Ready => "جاهز",
                        OpticalWorkOrderStatus.Delivered => "تم التسليم",
                        OpticalWorkOrderStatus.Cancelled => "ملغي",
                        _ => "-"
                    },
                    IsUrgent = x.IsUrgent,
                    IsOverdue = x.Status != OpticalWorkOrderStatus.Delivered &&
                                x.Status != OpticalWorkOrderStatus.Cancelled &&
                                x.ExpectedDeliveryDateUtc.HasValue &&
                                x.ExpectedDeliveryDateUtc.Value < DateTime.UtcNow
                })
                .ToList()
        };

        ViewBag.Filter = filter ?? "all";
        return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> Create(Guid salesInvoiceId)
    {
        var invoice = await _salesInvoiceRepository.GetByIdAsync(salesInvoiceId);
        if (invoice == null)
            return NotFound();

        var vm = new CreateOpticalWorkOrderVm
        {
            SalesInvoiceId = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerName = invoice.CustomerName,
            PrescriptionText = invoice.Prescription != null
                ? invoice.Prescription.PrescriptionDateUtc.ToLocalTime().ToString("yyyy-MM-dd")
                : null,
            WorkOrderNumber = "WO-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            OrderDateUtc = DateTime.UtcNow,
            ExpectedDeliveryDateUtc = DateTime.UtcNow.AddDays(3)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOpticalWorkOrderVm vm)
    {
        var invoice = await _salesInvoiceRepository.GetByIdAsync(vm.SalesInvoiceId);
        if (invoice == null)
            return NotFound();

        vm.InvoiceNumber = invoice.InvoiceNumber;
        vm.CustomerName = invoice.CustomerName;
        vm.PrescriptionText = invoice.Prescription != null
            ? invoice.Prescription.PrescriptionDateUtc.ToLocalTime().ToString("yyyy-MM-dd")
            : null;

        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var id = await _opticalWorkOrderService.CreateAsync(vm);
            TempData["Success"] = "تم إنشاء أمر الشغل بنجاح";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _opticalWorkOrderRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new OpticalWorkOrderDetailsVm
        {
            Id = entity.Id,
            WorkOrderNumber = entity.WorkOrderNumber,
            InvoiceNumber = entity.SalesInvoice?.InvoiceNumber ?? "-",
            CustomerName = entity.Customer?.NameAr ?? entity.SalesInvoice?.CustomerName ?? "-",
            PrescriptionText = entity.Prescription != null
                ? entity.Prescription.PrescriptionDateUtc.ToLocalTime().ToString("yyyy-MM-dd")
                : null,
            OrderDateUtc = entity.OrderDateUtc,
            ExpectedDeliveryDateUtc = entity.ExpectedDeliveryDateUtc,
            ReadyDateUtc = entity.ReadyDateUtc,
            DeliveredDateUtc = entity.DeliveredDateUtc,
            StatusText = entity.Status.ToString(),
            IsUrgent = entity.IsUrgent,
            FrameNotes = entity.FrameNotes,
            LensNotes = entity.LensNotes,
            WorkshopNotes = entity.WorkshopNotes,
            DeliveryNotes = entity.DeliveryNotes,
            Items = entity.SalesInvoice?.Lines.Select(x => new OpticalWorkOrderLineVm
            {
                ItemCode = x.Item?.ItemCode ?? "",
                ItemNameAr = x.Item?.ItemNameAr ?? "",
                Quantity = x.Quantity
            }).ToList() ?? new List<OpticalWorkOrderLineVm>()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkInLab(Guid id)
    {
        await _opticalWorkOrderService.MarkInLabAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkReady(Guid id)
    {
        await _opticalWorkOrderService.MarkReadyAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkDelivered(Guid id)
    {
        await _opticalWorkOrderService.MarkDeliveredAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }
}