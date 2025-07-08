using System.ComponentModel.DataAnnotations;
using pet_spa_system1.Models;
using System;
using System.Collections.Generic;

namespace pet_spa_system1.ViewModels
{
    public class AppointmentViewModel
    {
        public List<Models.Service> Services { get; set; } = new();
        public List<Models.SerCate> Categories { get; set; } = new();
        public List<Pet> Pets { get; set; } = new();

        public int UserId { get; set; }
        public int? EmployeeId { get; set; }
        
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public List<int> SelectedServiceIds { get; set; } = new List<int>();
        public List<int> SelectedPetIds { get; set; } = new List<int>();
        public string? Notes { get; set; }
        
        // Additional properties for email template
        public List<Pet> SelectedPets { get; set; } = new List<Pet>();
        public List<Models.Service> SelectedServices { get; set; } = new List<Models.Service>();
    }
}