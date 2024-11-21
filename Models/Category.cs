namespace CoolShopFlowers.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Навігаційні властивості
        public ICollection<Flower> Flowers { get; set; } = new List<Flower>();
    }

}
