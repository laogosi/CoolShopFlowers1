using CoolShopFlowers.Models;

namespace CoolShopFlowers.Repos.Abstract
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bicycleId, int qty, string userId);
        Task<int> RemoveItem(int bicycleId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(string userId = "");
        Task<ShoppingCart> GetCart(string userId);
        Task<bool> DoCheckout(string userId);
        Task<List<Order>> GetUserOrders();
        Task<Order?> GetOrderById(int orderId);
    }
}
