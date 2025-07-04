using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModels;

public class AppointmentDashboardViewModel
{
    public int TodayCount { get; set; }
    public int PendingCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public List<AppointmentHistoryItemViewModel> Appointments { get; set; } = new();
    // ... các property khác
}
