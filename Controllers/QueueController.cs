using LoadingQueue.Models;
using LoadingQueue.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LoadingQueue.Controllers;

public class QueueController : BaseController
{
    private readonly IQueueRepository _queueRepository;
    private readonly IDriverRepository _driverRepository;

    private readonly INotificationRepository _notificationRepository;
    private readonly ISettingsRepository _settingsRepository;

    public QueueController(
        IQueueRepository queueRepository,
        IDriverRepository driverRepository,
        INotificationRepository notificationRepository,
    ISettingsRepository settingsRepository,
        Microsoft.AspNetCore.Identity.UserManager<Models.ApplicationUser> userManager)
        : base(userManager)
    {
        _queueRepository = queueRepository;
        _driverRepository = driverRepository;
        _notificationRepository = notificationRepository;
        _settingsRepository = settingsRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    #region GetData

    [HttpGet]
    public async Task<IActionResult> GetData(DateTime? date)
    {
        var companyId = await GetCurrentCompanyIdAsync();
        var data = await _queueRepository.GetByDate(date ?? DateTime.Today, companyId);
        return Json(data.Select(q => new
        {
            id = q.Id,

            shippingCompanyName = q.ShippingCompanyName,

            waybillNumber = q.WaybillNumber,

            driverCardNumber = q.DriverCardNumber,

            driverName = q.DriverName,

            freightAmount = q.FreightAmount,

            destination = q.Destination,

            driverCarNumber = q.DriverCarNumber,

            queueTime = q.QueueTime?.ToString(@"hh\:mm"),

            cakeCartonCount = q.CakeCartonCount,

            nutCartonCount = q.NutCartonCount,

            exitNumber = q.ExitNumber,

            driverPhone = q.DriverPhone,

            queueNumber = q.QueueNumber,

            status = (int)q.Status,

            statusText = GetStatusText(q.Status)
        }));
    }

    #endregion


    private static string GetStatusText(QueueStatus status) => status switch
    {
        QueueStatus.Pending => "در انتظار",
        QueueStatus.InProgress => "در حال بارگیری",
        QueueStatus.Completed => "تکمیل شده",
        QueueStatus.Cancelled => "لغو شده",
        _ => "-"
    };

    #region Update Cell

    [HttpPost]
    public async Task<IActionResult> UpdateCell(int id, string field, string value)
    {
        var companyId = await GetCurrentCompanyIdAsync();

        var queue = await _queueRepository.GetByIdAsync(id, companyId);
        if (queue == null)
            return BadRequest();

        switch (field)
        {
            case "shippingCompanyName":
                queue.ShippingCompanyName = value;
                break;

            case "waybillNumber":
                queue.WaybillNumber = value;
                break;

            case "driverCardNumber":
                queue.DriverCardNumber = value;
                break;

            case "driverName":
                queue.DriverName = value;
                break;

            case "destination":
                queue.Destination = value;
                break;

            case "driverCarNumber":
                queue.DriverCarNumber = value;
                break;

            case "driverPhone":
                queue.DriverPhone = value;
                break;

            case "exitNumber":
                queue.ExitNumber = value;
                break;

            case "freightAmount":
                queue.FreightAmount =
                    string.IsNullOrWhiteSpace(value)
                    ? null
                    : decimal.Parse(value);
                break;

            case "cakeCartonCount":
                queue.CakeCartonCount =
                    string.IsNullOrWhiteSpace(value)
                    ? null
                    : int.Parse(value);
                break;

            case "nutCartonCount":
                queue.NutCartonCount =
                    string.IsNullOrWhiteSpace(value)
                    ? null
                    : int.Parse(value);
                break;

            case "queueTime":
                queue.QueueTime = TimeSpan.Parse(value);
                break;
        }

        await _queueRepository.UpdateAsync(queue);

        return Ok();
    }

    #endregion


    #region Change Status

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int id, int status)
    {
        var companyId = await GetCurrentCompanyIdAsync();

        if (status == (int)QueueStatus.Completed && companyId.HasValue && !User.IsInRole("SystemAdmin") && !User.IsInRole("CompanyAdmin"))
        {
            var settings = await _settingsRepository.GetOrCreateAsync(companyId.Value);

            if (settings.RequireApprovalForCompletion)
            {
                // کاربر عادی اجازه نداره، ولی به مدیر شرکت خبر می‌دیم که
                // یه نوبت منتظر تاییده
                var pendingQueue = await _queueRepository.GetByIdAsync(id, companyId);

                if (pendingQueue != null)
                {
                    await _notificationRepository.AddAsync(
                        companyId.Value,
                        "درخواست تایید تکمیل نوبت",
                        $"نوبت {pendingQueue.QueueNumber} منتظر تایید شماست تا به‌عنوان تکمیل‌شده ثبت شود.");
                }

                return Forbid();
            }
        }

        var ok = await _queueRepository.ChangeStatusAsync(id, (QueueStatus)status, companyId);

        if (!ok)
            return NotFound();

        if (status == (int)QueueStatus.Completed && companyId.HasValue)
        {
            var completedQueue = await _queueRepository.GetByIdAsync(id, companyId);

            if (completedQueue != null)
            {
                await _notificationRepository.AddAsync(
                    companyId.Value,
                    "نوبت تکمیل شد",
                    $"نوبت {completedQueue.QueueNumber} با موفقیت تکمیل شد.");
            }
        }

        return Ok();
    }

    #endregion

    #region Reorder

    [HttpPost]
    public async Task<IActionResult> Reorder([FromBody] List<int> ids)
    {
        await _queueRepository.ReorderAsync(ids);
        return Ok();
    }

    #endregion

    #region Create Empty Queue

    [HttpPost]
    public async Task<IActionResult> CreateEmpty(int driverId, DateTime? date)
    {
        try
        {
            var companyId = await GetCurrentCompanyIdAsync();

            var driver = await _driverRepository.GetByIdAsync(driverId, companyId);

            if (driver == null)
            {
                return Json(new { success = false, message = "راننده پیدا نشد." });
            }

            var queue = new Queue
            {
                CompanyId = driver.CompanyId,

                DriverId = driver.Id,
                DriverName = driver.FullName,
                DriverPhone = driver.PhoneNumber,
                DriverCarNumber = driver.CarNumber,
                DriverCardNumber = driver.CardNumberOrSheba,

                ShippingCompanyName = "",
                WaybillNumber = "",
                Destination = "",
                ExitNumber = "",
                FreightAmount = null,
                CakeCartonCount = null,
                NutCartonCount = null,

                QueueDate = date ?? DateTime.Today,
                QueueTime = DateTime.Now.TimeOfDay
            };

            await _queueRepository.AddAsync(queue);

            await _notificationRepository.AddAsync(
                driver.CompanyId,
                "نوبت جدید ثبت شد",
                $"نوبت {queue.QueueNumber} برای راننده {driver.FullName} ثبت شد.");

            return Json(new { success = true, id = queue.Id });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message, detail = ex.InnerException?.Message });
        }
    }

    #endregion

    #region Delete

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _queueRepository.DeleteAsync(id);

        return Ok();
    }

    #endregion
}