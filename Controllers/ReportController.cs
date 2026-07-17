using ClosedXML.Excel;
using LoadingQueue.Models;
using LoadingQueue.Repositories;
using LoadingQueue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoadingQueue.Controllers;

[Authorize(Roles = "SystemAdmin,CompanyAdmin,User")]
public class ReportController : BaseController
{
    private readonly IQueueRepository _queueRepository;

    public ReportController(IQueueRepository queueRepository, UserManager<ApplicationUser> userManager)
        : base(userManager)
    {
        _queueRepository = queueRepository;
    }

    public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, int? status, string? search)
    {
        var companyId = await GetCurrentCompanyIdAsync();

        var from = fromDate ?? DateTime.Today.AddDays(-6);
        var to = toDate ?? DateTime.Today;

        var results = await _queueRepository.SearchAsync(
            from, to,
            status.HasValue ? (QueueStatus)status.Value : null,
            search,
            companyId);

        var vm = new ReportFilterVm
        {
            FromDate = from,
            ToDate = to,
            Status = status,
            Search = search,
            Results = results
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(DateTime? fromDate, DateTime? toDate, int? status, string? search)
    {
        var companyId = await GetCurrentCompanyIdAsync();

        var from = fromDate ?? DateTime.Today.AddDays(-6);
        var to = toDate ?? DateTime.Today;

        var results = await _queueRepository.SearchAsync(
            from, to,
            status.HasValue ? (QueueStatus)status.Value : null,
            search,
            companyId);

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("گزارش نوبت‌ها");
        sheet.RightToLeft = true;

        string[] headers = { "شماره نوبت", "تاریخ", "ساعت", "باربری", "راننده", "شماره ماشین", "شماره بارنامه", "مقصد", "کرایه (ریال)", "وضعیت" };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = sheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0d6efd");
            cell.Style.Font.FontColor = XLColor.White;
        }

        var pc = new System.Globalization.PersianCalendar();
        int row = 2;

        foreach (var q in results)
        {
            sheet.Cell(row, 1).Value = q.QueueNumber;
            sheet.Cell(row, 2).Value = $"{pc.GetYear(q.QueueDate):0000}/{pc.GetMonth(q.QueueDate):00}/{pc.GetDayOfMonth(q.QueueDate):00}";
            sheet.Cell(row, 3).Value = q.QueueTime?.ToString(@"hh\:mm");
            sheet.Cell(row, 4).Value = q.ShippingCompanyName;
            sheet.Cell(row, 5).Value = q.DriverName;
            sheet.Cell(row, 6).Value = q.DriverCarNumber;
            sheet.Cell(row, 7).Value = q.WaybillNumber;
            sheet.Cell(row, 8).Value = q.Destination;
            sheet.Cell(row, 9).Value = (double)(q.FreightAmount ?? 0);
            sheet.Cell(row, 10).Value = GetStatusText(q.Status);
            row++;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "QueueReport.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> Print(DateTime? fromDate, DateTime? toDate, int? status, string? search)
    {
        var companyId = await GetCurrentCompanyIdAsync();

        var from = fromDate ?? DateTime.Today.AddDays(-6);
        var to = toDate ?? DateTime.Today;

        var results = await _queueRepository.SearchAsync(
            from, to,
            status.HasValue ? (QueueStatus)status.Value : null,
            search,
            companyId);

        var vm = new ReportFilterVm
        {
            FromDate = from,
            ToDate = to,
            Status = status,
            Search = search,
            Results = results
        };

        return View(vm);
    }

    private static string GetStatusText(QueueStatus status) => status switch
    {
        QueueStatus.Pending => "در انتظار",
        QueueStatus.InProgress => "در حال بارگیری",
        QueueStatus.Completed => "تکمیل شده",
        QueueStatus.Cancelled => "لغو شده",
        _ => "-"
    };
}