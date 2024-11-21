using System.ComponentModel.DataAnnotations;

namespace CoolShopFlowers.Models
{
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        public int ShoppingCartId { get; set; }
        [Required]
        public int FlowerId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double Price { get; set; }

        public Flower Flower { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
