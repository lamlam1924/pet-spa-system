using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models
{
    public class Pet
    {
        public int PetId { get; set; }
        public string Name { get; set; }
        public string? Breed { get; set; }
        public int? SpeciesId { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? HealthCondition { get; set; }
        public string? SpecialNotes { get; set; }
        public DateTime? LastSpaVisit { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UserId { get; set; } // Có thể null hoặc [Required] tùy database

        public Species? Species { get; set; }
        public User? User { get; set; }
        public List<AppointmentPet>? AppointmentPets { get; set; }
    }
}