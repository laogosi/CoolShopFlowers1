using CoolShopFlowers.Models;
using CoolShopFlowers.Repos.Abstract;
using CoolShopFlowers.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoolShopFlowers.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;
        private readonly UserManager<User> _userManager;
        private readonly IUserAuthService _service;
        private readonly HttpClient _httpClient;
        
        public CartController(ICartRepository cartRepo, UserManager<User> userManager, IUserAuthService service, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _userManager = userManager;
            _service = service; 
            _cartRepo = cartRepo;
        }

        public async Task<IActionResult> AddItem(int Id, int qty, int redirect = 0)
        {
            // Отримуємо UserId
            var userId = _userManager.GetUserId(User);

            // Перевірка, чи є користувач в системі
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in");
            }

            // Додаємо товар в кошик
            var cartCount = await _cartRepo.AddItem(Id, qty, userId);

            if (redirect == 0)
                return RedirectToAction("GetUserCart");

            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int Id)
        {
            var cartCount = await _cartRepo.RemoveItem(Id);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart); // Відображення в GetUserCart.cshtml
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetCartItemCount();
            return Ok(cartItem);
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);

            try
            {
                var isCheckedOut = await _cartRepo.DoCheckout(userId);
                if (!isCheckedOut)
                {
                    throw new Exception("An error occurred while processing your order.");
                }

                return RedirectToAction("OrderSuccess");
            }
            catch (Exception ex)
            {
                // Виведення помилки в консоль
                Console.WriteLine($"Error during checkout: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        public async Task<IActionResult> UserOrders()
        {
            var orders = await _cartRepo.GetUserOrders();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var order = await _cartRepo.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            return View(order);
        }

    }
}
