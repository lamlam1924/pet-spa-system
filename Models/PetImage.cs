namespace pet_spa_system1.Models
{
    public class PetImage
    {
        public int PetImageId { get; set; }
        public int PetId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        // Quan hệ với Pet
        public virtual Pet Pet { get; set; }
    }
}