using CoolShopFlowers.Models;
using CoolShopFlowers.Models.DTO;
using CoolShopFlowers.Repos.Abstract;
using CoolShopFlowers.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoolShopFlowers.Controllers
{
    public class UserController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUserAuthService _service;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserController(DBContext context, IConfiguration configuration, HttpClient httpClient, IUserAuthService service, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _service = service;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public IActionResult Registr()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registr(Registr model)
        {
            model.Role = "user";
            model.IsBanned = false;
            model.IsMuted = false;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    var result = await _service.RegistrAsync(model);
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("", "Email already in use");
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Wrong password or/and login");
                return View(model);
            }

            var result = await _service.LoginAsync(model);

            if (result.StatusCode == 1)
            {
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                await _userManager.UpdateAsync(user);
                ModelState.AddModelError("", "Невірний логін або пароль.");
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users.Count == 0)
            {
                ViewBag.Message = "...";
            }

            var model = users.Select(user => new ListUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UserListSearch(string searchString)
        {
            var users = await _userManager.Users.ToListAsync();

            // User filter based on search request
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    u.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                if (users.Count == 0)
                {
                    ViewBag.Message = "Такого користувача немає";
                }
            }

            var model = users.Select(user => new ListUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name
            }).ToList();

            return View("UserList", model);
        }

        public async Task<IActionResult> Account(Guid? id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                BirthDay = user.BirthDay,
                Sex = user.Sex,
                UserName = user.UserName,
                IsBanned = user.IsBanned,
                IsMuted = user.IsMuted,
            };

            if (user.BirthDay != null)
            {
                userViewModel.Age = CalculateAge(user.BirthDay);
            }

            return View(userViewModel);
        }
        private int CalculateAge(DateTime birthDay)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDay.Year;

            if (birthDay.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Ban(BanUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.id);
            if (user == null) return NotFound();

            if (!user.IsBanned) user.IsBanned = true;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnBan(BanUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.id);
            if (user == null) return NotFound();

            if (user.IsBanned) user.IsBanned = false;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Mute(MuteUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.id);
            if (user == null) return NotFound();

            if (!user.IsMuted) user.IsMuted = true;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnMute(MuteUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.id);
            if (user == null) return NotFound();

            if (user.IsMuted) user.IsMuted = false;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var model = new UpdPasswordViewModel
            {
                Id = user.Id
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, UpdPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            // Cheack current user password
            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!passwordCheck)
            {
                ModelState.AddModelError(string.Empty, "Неправильний поточний пароль.");
                return View(model);
            }

            // Changing password
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Message"] = "Пароль успішно змінено";
                return RedirectToAction("Edit", new { id = user.Id });
            }

            // For errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var model = new UpdUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                BirthDay = user.BirthDay,
                Sex = user.Sex,
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UpdUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}");
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($"Error: {subError.ErrorMessage}");
                    }
                }
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Name;
            user.BirthDay = model.BirthDay;
            user.Sex = model.Sex;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }
    }
}
