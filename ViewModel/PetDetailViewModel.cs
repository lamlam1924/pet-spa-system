using pet_spa_system1.Models;
using System.Collections.Generic;

namespace pet_spa_system1.ViewModel
{
    public class PetDetailViewModel
    {
        public Pet Pet { get; set; }

        public List<Pet> SuggestedPets { get; set; } = new List<Pet>(); // Mặc định rỗng, không bắt buộc

        public DateTime? LastSpaVisit { get; set; } // Không bắt buộc

        public int AppointmentCount { get; set; } = 0; // Mặc định 0, không bắt buộc

        public string SpeciesName { get; set; } // Không bắt buộc, có thể null

        public string OwnerName { get; set; } // Không bắt buộc, có thể null

        public bool IsActive { get; set; } = true; // Mặc định true, không bắt buộc

        public List<Species> SpeciesList { get; set; } = new List<Species>(); // Mặc định rỗng, không bắt buộc
    }
}