using pet_spa_system1.Models;
using System;
using System.Collections.Generic;

namespace pet_spa_system1.ViewModels;

public class AppointmentHistoryItemViewModel
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; }
    public List<string> PetNames { get; set; }
    public string Notes { get; set; }
    public List<ServiceHistoryInfo> Services { get; set; } = new();
    // Đảm bảo ViewModel có định nghĩa:
}

public class AppointmentHistoryViewModel
{
    public List<AppointmentHistoryItemViewModel> Appointments { get; set; } = new();
    public List<StatusAppointment> Statuses { get; set; } = new();
}

public class ServiceHistoryInfo
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
}
