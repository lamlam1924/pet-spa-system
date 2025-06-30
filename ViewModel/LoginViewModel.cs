using System.ComponentModel.DataAnnotations;

namespace pet_spa_system1.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public required string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}