using System.ComponentModel.DataAnnotations;

namespace pet_spa_system1.Models;

public class AppointmentViewModel
{
    public List<Service> Services { get; set; } = new();
    public List<SerCate> Categories { get; set; } = new();
    public List<Pet> Pets { get; set; } = new();

    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public List<int> SelectedServiceIds { get; set; } = new List<int>();
    public List<int> SelectedPetIds { get; set; } = new List<int>();
    public string? Notes { get; set; }
}
