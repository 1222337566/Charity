using ClosedXML.Excel;
using InfrastrfuctureManagmentCore.Queries.Reports;
using InfrastructureManagmentServices.CharityReports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace SInfrastructureManagmentServices.CharityReports;

public class CharityReportExportService : ICharityReportExportService
{
    static CharityReportExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] ExportBeneficiaryStatusesExcel(IEnumerable<BeneficiaryStatusReportRowDto> rows)
    {
        using var workbook = CreateWorkbook("Beneficiary Statuses", ws =>
        {
            ws.Cell(1, 1).Value = "الحالة";
            ws.Cell(1, 2).Value = "عدد المستفيدين";
            ws.Cell(1, 3).Value = "طلبات المساعدة";
            ws.Cell(1, 4).Value = "مرات الصرف";
            ws.Cell(1, 5).Value = "إجمالي الصرف";

            var rowIndex = 2;
            foreach (var row in rows)
            {
                ws.Cell(rowIndex, 1).Value = row.StatusName;
                ws.Cell(rowIndex, 2).Value = row.BeneficiariesCount;
                ws.Cell(rowIndex, 3).Value = row.AidRequestsCount;
                ws.Cell(rowIndex, 4).Value = row.DisbursementsCount;
                ws.Cell(rowIndex, 5).Value = row.TotalDisbursedAmount;
                rowIndex++;
            }

            ws.Cell(rowIndex, 1).Value = "الإجمالي";
            ws.Cell(rowIndex, 2).FormulaA1 = $"SUM(B2:B{rowIndex - 1})";
            ws.Cell(rowIndex, 3).FormulaA1 = $"SUM(C2:C{rowIndex - 1})";
            ws.Cell(rowIndex, 4).FormulaA1 = $"SUM(D2:D{rowIndex - 1})";
            ws.Cell(rowIndex, 5).FormulaA1 = $"SUM(E2:E{rowIndex - 1})";
        });

        return SaveWorkbook(workbook);
    }

    public byte[] ExportBeneficiaryStatusesPdf(IEnumerable<BeneficiaryStatusReportRowDto> rows)
    {
        return BuildPdf(
            "تقرير حالات المستفيدين",
            null,
            new[] { "الحالة", "عدد المستفيدين", "طلبات المساعدة", "مرات الصرف", "إجمالي الصرف" },
            rows.Select(x => new[]
            {
                x.StatusName,
                x.BeneficiariesCount.ToString(),
                x.AidRequestsCount.ToString(),
                x.DisbursementsCount.ToString(),
                x.TotalDisbursedAmount.ToString("N2")
            }));
    }

    public byte[] ExportDonationsExcel(IEnumerable<DonationReportRowDto> rows, DateTime? fromDate, DateTime? toDate)
    {
        using var workbook = CreateWorkbook("Donations", ws =>
        {
            WritePeriod(ws, fromDate, toDate);

            ws.Cell(3, 1).Value = "التاريخ";
            ws.Cell(3, 2).Value = "رقم التبرع";
            ws.Cell(3, 3).Value = "المتبرع";
            ws.Cell(3, 4).Value = "النوع";
            ws.Cell(3, 5).Value = "القيمة";
            ws.Cell(3, 6).Value = "طريقة الدفع";
            ws.Cell(3, 7).Value = "الحساب";
            ws.Cell(3, 8).Value = "مقيد";
            ws.Cell(3, 9).Value = "الحملة";

            var rowIndex = 4;
            foreach (var row in rows)
            {
                ws.Cell(rowIndex, 1).Value = row.DonationDate;
                ws.Cell(rowIndex, 1).Style.DateFormat.Format = "yyyy-mm-dd";
                ws.Cell(rowIndex, 2).Value = row.DonationNumber;
                ws.Cell(rowIndex, 3).Value = row.DonorName;
                ws.Cell(rowIndex, 4).Value = row.DonationType;
                ws.Cell(rowIndex, 5).Value = row.Amount;
                ws.Cell(rowIndex, 6).Value = row.PaymentMethodName;
                ws.Cell(rowIndex, 7).Value = row.FinancialAccountName;
                ws.Cell(rowIndex, 8).Value = row.IsRestricted ? "نعم" : "لا";
                ws.Cell(rowIndex, 9).Value = row.CampaignName ?? string.Empty;
                rowIndex++;
            }

            ws.Cell(rowIndex, 4).Value = "الإجمالي";
            ws.Cell(rowIndex, 5).FormulaA1 = $"SUM(E4:E{rowIndex - 1})";
        });

        return SaveWorkbook(workbook);
    }

    public byte[] ExportDonationsPdf(IEnumerable<DonationReportRowDto> rows, DateTime? fromDate, DateTime? toDate)
    {
        return BuildPdf(
            "تقرير التبرعات",
            BuildPeriodText(fromDate, toDate),
            new[] { "التاريخ", "رقم التبرع", "المتبرع", "النوع", "القيمة", "طريقة الدفع", "الحساب", "مقيد" },
            rows.Select(x => new[]
            {
                x.DonationDate.ToString("yyyy-MM-dd"),
                x.DonationNumber,
                x.DonorName,
                x.DonationType,
                x.Amount.ToString("N2"),
                x.PaymentMethodName,
                x.FinancialAccountName,
                x.IsRestricted ? "نعم" : "لا"
            }));
    }

    public byte[] ExportProjectsExcel(IEnumerable<ProjectFinancialReportRowDto> rows)
    {
        using var workbook = CreateWorkbook("Projects", ws =>
        {
            ws.Cell(1, 1).Value = "الكود";
            ws.Cell(1, 2).Value = "المشروع";
            ws.Cell(1, 3).Value = "الحالة";
            ws.Cell(1, 4).Value = "الموازنة";
            ws.Cell(1, 5).Value = "بنود مخططة";
            ws.Cell(1, 6).Value = "بنود فعلية";
            ws.Cell(1, 7).Value = "تمويل مخصص";
            ws.Cell(1, 8).Value = "المستفيدون";
            ws.Cell(1, 9).Value = "الأنشطة";

            var rowIndex = 2;
            foreach (var row in rows)
            {
                ws.Cell(rowIndex, 1).Value = row.ProjectCode;
                ws.Cell(rowIndex, 2).Value = row.ProjectName;
                ws.Cell(rowIndex, 3).Value = row.Status;
                ws.Cell(rowIndex, 4).Value = row.Budget;
                ws.Cell(rowIndex, 5).Value = row.PlannedBudgetLines;
                ws.Cell(rowIndex, 6).Value = row.ActualBudgetLines;
                ws.Cell(rowIndex, 7).Value = row.AllocatedGrants;
                ws.Cell(rowIndex, 8).Value = row.BeneficiariesCount;
                ws.Cell(rowIndex, 9).Value = row.ActivitiesCount;
                rowIndex++;
            }
        });

        return SaveWorkbook(workbook);
    }

    public byte[] ExportProjectsPdf(IEnumerable<ProjectFinancialReportRowDto> rows)
    {
        return BuildPdf(
            "تقرير المشروعات",
            null,
            new[] { "الكود", "المشروع", "الحالة", "الموازنة", "بنود مخططة", "بنود فعلية", "تمويل مخصص", "المستفيدون", "الأنشطة" },
            rows.Select(x => new[]
            {
                x.ProjectCode,
                x.ProjectName,
                x.Status,
                x.Budget.ToString("N2"),
                x.PlannedBudgetLines.ToString("N2"),
                x.ActualBudgetLines.ToString("N2"),
                x.AllocatedGrants.ToString("N2"),
                x.BeneficiariesCount.ToString(),
                x.ActivitiesCount.ToString()
            }));
    }

    public byte[] ExportPayrollExcel(IEnumerable<PayrollMonthReportRowDto> rows)
    {
        using var workbook = CreateWorkbook("Payroll", ws =>
        {
            ws.Cell(1, 1).Value = "الشهر";
            ws.Cell(1, 2).Value = "الحالة";
            ws.Cell(1, 3).Value = "عدد الموظفين";
            ws.Cell(1, 4).Value = "الأساسي";
            ws.Cell(1, 5).Value = "الإضافات";
            ws.Cell(1, 6).Value = "الخصومات";
            ws.Cell(1, 7).Value = "الصافي";

            var rowIndex = 2;
            foreach (var row in rows)
            {
                ws.Cell(rowIndex, 1).Value = $"{row.Month:00}/{row.Year}";
                ws.Cell(rowIndex, 2).Value = row.Status;
                ws.Cell(rowIndex, 3).Value = row.EmployeesCount;
                ws.Cell(rowIndex, 4).Value = row.TotalBasic;
                ws.Cell(rowIndex, 5).Value = row.TotalAdditions;
                ws.Cell(rowIndex, 6).Value = row.TotalDeductions;
                ws.Cell(rowIndex, 7).Value = row.TotalNet;
                rowIndex++;
            }

            ws.Cell(rowIndex, 3).Value = "الإجمالي";
            ws.Cell(rowIndex, 4).FormulaA1 = $"SUM(D2:D{rowIndex - 1})";
            ws.Cell(rowIndex, 5).FormulaA1 = $"SUM(E2:E{rowIndex - 1})";
            ws.Cell(rowIndex, 6).FormulaA1 = $"SUM(F2:F{rowIndex - 1})";
            ws.Cell(rowIndex, 7).FormulaA1 = $"SUM(G2:G{rowIndex - 1})";
        });

        return SaveWorkbook(workbook);
    }

    public byte[] ExportPayrollPdf(IEnumerable<PayrollMonthReportRowDto> rows)
    {
        return BuildPdf(
            "تقرير المرتبات",
            null,
            new[] { "الشهر", "الحالة", "عدد الموظفين", "الأساسي", "الإضافات", "الخصومات", "الصافي" },
            rows.Select(x => new[]
            {
                $"{x.Month:00}/{x.Year}",
                x.Status,
                x.EmployeesCount.ToString(),
                x.TotalBasic.ToString("N2"),
                x.TotalAdditions.ToString("N2"),
                x.TotalDeductions.ToString("N2"),
                x.TotalNet.ToString("N2")
            }));
    }

    public byte[] ExportStoreMovementsExcel(IEnumerable<StoreMovementReportRowDto> rows, DateTime? fromDate, DateTime? toDate)
    {
        using var workbook = CreateWorkbook("Store Movements", ws =>
        {
            WritePeriod(ws, fromDate, toDate);

            ws.Cell(3, 1).Value = "المخزن";
            ws.Cell(3, 2).Value = "عدد أذون الإضافة";
            ws.Cell(3, 3).Value = "كمية الإضافة";
            ws.Cell(3, 4).Value = "عدد أذون الصرف";
            ws.Cell(3, 5).Value = "كمية الصرف";

            var rowIndex = 4;
            foreach (var row in rows)
            {
                ws.Cell(rowIndex, 1).Value = row.WarehouseName;
                ws.Cell(rowIndex, 2).Value = row.ReceiptsCount;
                ws.Cell(rowIndex, 3).Value = row.ReceiptQuantity;
                ws.Cell(rowIndex, 4).Value = row.IssuesCount;
                ws.Cell(rowIndex, 5).Value = row.IssueQuantity;
                rowIndex++;
            }

            ws.Cell(rowIndex, 1).Value = "الإجمالي";
            ws.Cell(rowIndex, 3).FormulaA1 = $"SUM(C4:C{rowIndex - 1})";
            ws.Cell(rowIndex, 5).FormulaA1 = $"SUM(E4:E{rowIndex - 1})";
        });

        return SaveWorkbook(workbook);
    }

    public byte[] ExportStoreMovementsPdf(IEnumerable<StoreMovementReportRowDto> rows, DateTime? fromDate, DateTime? toDate)
    {
        return BuildPdf(
            "تقرير حركة المخازن",
            BuildPeriodText(fromDate, toDate),
            new[] { "المخزن", "عدد أذون الإضافة", "كمية الإضافة", "عدد أذون الصرف", "كمية الصرف" },
            rows.Select(x => new[]
            {
                x.WarehouseName,
                x.ReceiptsCount.ToString(),
                x.ReceiptQuantity.ToString("N2"),
                x.IssuesCount.ToString(),
                x.IssueQuantity.ToString("N2")
            }));
    }

    private static XLWorkbook CreateWorkbook(string sheetName, Action<IXLWorksheet> fill)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);
        fill(worksheet);
        var used = worksheet.RangeUsed();
        if (used is not null)
        {
            used.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            used.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            used.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);
            used.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            used.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        worksheet.Columns().AdjustToContents();
        worksheet.RightToLeft = true;
        return workbook;
    }

    private static byte[] SaveWorkbook(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void WritePeriod(IXLWorksheet worksheet, DateTime? fromDate, DateTime? toDate)
    {
        worksheet.Cell(1, 1).Value = "من";
        worksheet.Cell(1, 2).Value = fromDate?.ToString("yyyy-MM-dd") ?? "الكل";
        worksheet.Cell(2, 1).Value = "إلى";
        worksheet.Cell(2, 2).Value = toDate?.ToString("yyyy-MM-dd") ?? "الكل";
    }

    private static string? BuildPeriodText(DateTime? fromDate, DateTime? toDate)
    {
        if (fromDate is null && toDate is null)
            return null;

        return $"الفترة: {(fromDate?.ToString("yyyy-MM-dd") ?? "الكل")} → {(toDate?.ToString("yyyy-MM-dd") ?? "الكل")}";
    }

    private static byte[] BuildPdf(string title, string? subtitle, IReadOnlyList<string> headers, IEnumerable<string[]> rows)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.A4.Landscape());
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().AlignCenter().Text(title).SemiBold().FontSize(16);
                    if (!string.IsNullOrWhiteSpace(subtitle))
                        column.Item().AlignCenter().Text(subtitle).FontSize(10).FontColor(Colors.Grey.Darken2);
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var _ in headers)
                            columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        foreach (var titleCell in headers)
                        {
                            header.Cell().Element(CellStyle).Text(titleCell).SemiBold();
                        }
                    });

                    foreach (var row in rows)
                    {
                        foreach (var cell in row)
                        {
                            table.Cell().Element(CellStyle).Text(cell ?? string.Empty);
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("صفحة ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();

        static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Padding(4)
                .AlignCenter()
                .AlignMiddle();
        }
    }
}
