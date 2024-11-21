using CoolShopFlowers.Models;
using CoolShopFlowers.Models.ViewModels;
using CoolShopFlowers.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoolShopFlowers.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;

        public HomeController(ILogger<HomeController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string query = null, string flowerType = null)
        {
            // Отримання категорій
            var categories = await _context.Categories.Include(c => c.Flowers).ToListAsync();

            // Пошук квітів
            var flowersQuery = _context.Flowers
                .Include(f => f.Category)
                .AsQueryable();

            // Фільтр за назвою
            if (!string.IsNullOrEmpty(query))
            {
                flowersQuery = flowersQuery.Where(f => f.Name.Contains(query));
            }

            // Фільтр за типом квітки
            if (!string.IsNullOrEmpty(flowerType) && Enum.TryParse(flowerType, out FlowerType selectedType))
            {
                flowersQuery = flowersQuery.Where(f => f.FlowerType == selectedType);
            }

            // Отримання квітів
            var flowers = await flowersQuery.ToListAsync();

            // Створення ViewModel
            var viewModel = new FlowerAndCategoryViewModel
            {
                Categories = categories,
                Flowers = flowers
            };

            return View(viewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Admin()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) }
            );
            return LocalRedirect(returnUrl);
        }
    }
}
