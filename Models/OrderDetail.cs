namespace CoolShopFlowers.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }  // Зовнішній ключ
        public int FlowerId { get; set; }  // Зовнішній ключ
        public int Quantity { get; set; }
        public double Price { get; set; }

        // Навігаційні властивості
        public Order? Order { get; set; }
        public Flower? Flower { get; set; }
    }

}
