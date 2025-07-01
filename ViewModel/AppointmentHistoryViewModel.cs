namespace pet_spa_system1.Models;

public class AppointmentHistoryItemViewModel
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; }
    public List<string> ServiceNames { get; set; }
    public List<string> PetNames { get; set; }
    public string Notes { get; set; }
}
public class AppointmentHistoryViewModel
{
    public List<AppointmentHistoryItemViewModel> Appointments { get; set; }
    public List<StatusAppointment> Statuses { get; set; }
}
