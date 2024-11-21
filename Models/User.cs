using Microsoft.AspNetCore.Identity;

namespace CoolShopFlowers.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string? Sex { get; set; }

        public bool IsBanned { get; set; }
        public bool IsMuted { get; set; }
        public bool IsAdmin { get; set; }
        // Навігаційні властивості
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
