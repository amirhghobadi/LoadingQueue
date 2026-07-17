using LoadingQueue.Models;

namespace LoadingQueue.ViewModels;

public class ReportFilterVm
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int? Status { get; set; }
    public string? Search { get; set; }

    public List<Queue> Results { get; set; } = new();

    public int TotalCount => Results.Count;
    public int PendingCount => Results.Count(x => x.Status == QueueStatus.Pending);
    public int CompletedCount => Results.Count(x => x.Status == QueueStatus.Completed);
    public int CancelledCount => Results.Count(x => x.Status == QueueStatus.Cancelled);
    public decimal TotalFreight => Results.Sum(x => x.FreightAmount ?? 0);
}