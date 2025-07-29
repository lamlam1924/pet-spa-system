using pet_spa_system1.Models;

namespace pet_spa_system1.Utils;

public static class AppointmentExtensions
{
    public static int GetDurationMinutes(this Appointment appointment)
    {
        if (appointment == null) return 0;
        return appointment.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0;
    }

    public static DateTime GetEndTime(this Appointment appointment)
    {
        if (appointment == null) return DateTime.MinValue;
        return appointment.AppointmentDate.AddMinutes(appointment.GetDurationMinutes());
    }
}