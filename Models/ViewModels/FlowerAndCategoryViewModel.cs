namespace CoolShopFlowers.Models.ViewModels
{
    public class FlowerAndCategoryViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Flower> Flowers { get; set; }
        public FlowerType? SelectedFlowerType { get; set; } // Вибраний тип
    }
}
