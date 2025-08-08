using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models
{
    public partial class Pet
    {
        public int PetId { get; set; }
        public int? UserId { get; set; }
        public int? SpeciesId { get; set; }
        public string Name { get; set; } = null!;
        public string? Breed { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? HealthCondition { get; set; }
        public string? SpecialNotes { get; set; }
        public DateTime? LastSpaVisit { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<AppointmentPet> AppointmentPets { get; set; } = new List<AppointmentPet>();

        // Quan hệ với Species
        public virtual Species? Species { get; set; }

        // Quan hệ với User
        public virtual User? User { get; set; }

        // Quan hệ với PetImages (hỗ trợ nhiều ảnh)
        public virtual ICollection<PetImage> PetImages { get; set; } = new List<PetImage>();
    }
}