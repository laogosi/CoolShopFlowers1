using CoolShopFlowers.Models;
using CoolShopFlowers.Models.DTO;
using CoolShopFlowers.Models.ViewModels;
using CoolShopFlowers.Repos.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CoolShopFlowers.Repos.Implementation
{
    public class UserAuthService : IUserAuthService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<User> signInManager;

        public UserAuthService(UserManager<User> userManager,
            SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }
        public async Task<Status> LoginAsync(Login model)
        {
            var status = new Status();

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Invalid email";
                return status;
            }
            if (user.IsBanned)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User has been blocked";
                return status;
            }
            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.StatusMessage = "Invalid Password";
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);

            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.StatusMessage = "Logged in successfully";
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User is locked out";
            }
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Error on logging in";
            }

            return status;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrAsync(Registr model)
        {
            var status = new Status();

            // Перевірка наявності користувача з таким же імейлом
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Пошто зарезервована, спробуйте іншу";
                return status;
            }

            // Створення нового користувача
            User user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Email = model.Email,
                UserName = model.Username,
                Sex = model.Sex,
                BirthDay = model.BirthDay,
                EmailConfirmed = true,
                IsBanned = false,
                IsMuted = false,
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.StatusMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return status;
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            status.StatusMessage = "Ви успішно зареєструвалися";
            return status;
        }

        public async Task<Status> EditUserAsync(string userId, UserViewModel model)
        {
            var status = new Status();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User not found";
                return status;
            }

            user.Id = model.Id;
            user.Name = model.Name;
            user.UserName = model.UserName;
            user.Sex = model.Sex;
            user.BirthDay = model.BirthDay;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User update failed";
                return status;
            }

            status.StatusCode = 1;
            status.StatusMessage = "User updated successfully";
            return status;
        }
    }
}
