using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class PetDetailViewModel
    {
        public Pet Pet { get; set; } = new Pet(); // Đảm bảo khởi tạo để tránh null

        public List<Species> SpeciesList { get; set; } = new List<Species>();
        public List<Pet> SuggestedPets { get; set; } = new List<Pet>();
        public List<PetImage> PetImages { get; set; } = new List<PetImage>();

        public DateTime? LastSpaVisit { get; set; }
        public int AppointmentCount { get; set; }
        public string SpeciesName { get; set; } = "N/A";
        public string OwnerName { get; set; } = "N/A";
        public bool? IsActive { get; set; } = true;
    }
}