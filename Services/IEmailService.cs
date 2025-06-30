namespace pet_spa_system1.Services;

public interface IEmailService
{
    void SendBookingConfirmation(
        string toEmail,
        string customerName,
        DateTime appointmentDate,
        string? notes = null,
        List<string>? petNames = null,
        List<string>? serviceNames = null
    );
}
