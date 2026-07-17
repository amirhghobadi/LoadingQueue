using System.Diagnostics;
using System.Globalization;
using LoadingQueue.Models;
using LoadingQueue.Repositories;
using LoadingQueue.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoadingQueue.Controllers;

public class HomeController : BaseController
{
    private readonly IQueueRepository _queueRepository;

    public HomeController(IQueueRepository queueRepository, UserManager<ApplicationUser> userManager)
        : base(userManager)
    {
        _queueRepository = queueRepository;
    }

    public async Task<IActionResult> Index()
    {
        var companyId = await GetCurrentCompanyIdAsync();

        var todayQueues = await _queueRepository.GetByDate(DateTime.Today, companyId);
        var weekStats = await _queueRepository.GetLastNDaysStatsAsync(7, companyId);

        var pc = new PersianCalendar();
        var dayNames = new[] { "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" };

        var vm = new DashboardVm
        {
            TodayCount = todayQueues.Count,
            TodayCompleted = todayQueues.Count(x => x.Status == QueueStatus.Completed),
            TodayPending = todayQueues.Count(x => x.Status == QueueStatus.Pending),
            WeekCount = weekStats.Sum(x => x.Value.total),

            ChartLabels = weekStats.Keys
                .OrderBy(d => d)
                .Select(d => dayNames[(int)d.DayOfWeek])
                .ToList(),

            ChartTotal = weekStats.OrderBy(x => x.Key).Select(x => x.Value.total).ToList(),
            ChartCompleted = weekStats.OrderBy(x => x.Key).Select(x => x.Value.completed).ToList(),

            StatusLabels = new List<string> { "در انتظار", "در حال بارگیری", "تکمیل شده", "لغو شده" },

            StatusCounts = new List<int>
            {
                todayQueues.Count(x => x.Status == QueueStatus.Pending),
                todayQueues.Count(x => x.Status == QueueStatus.InProgress),
                todayQueues.Count(x => x.Status == QueueStatus.Completed),
                todayQueues.Count(x => x.Status == QueueStatus.Cancelled)
            }
        };

        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}