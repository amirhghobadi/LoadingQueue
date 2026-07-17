namespace LoadingQueue.ViewModels;

public class DashboardVm
{
    public int TodayCount { get; set; }
    public int TodayCompleted { get; set; }
    public int TodayPending { get; set; }
    public int WeekCount { get; set; }

    public List<string> ChartLabels { get; set; } = new();
    public List<int> ChartTotal { get; set; } = new();
    public List<int> ChartCompleted { get; set; } = new();

    public List<string> StatusLabels { get; set; } = new();
    public List<int> StatusCounts { get; set; } = new();
}