using System.ComponentModel.DataAnnotations;

namespace CoolShopFlowers.Models.DTO
{
    public class Login
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        public bool IsBanned { get; set; }
    }
}
