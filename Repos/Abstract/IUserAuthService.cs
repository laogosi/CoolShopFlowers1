using CoolShopFlowers.Models.DTO;
using CoolShopFlowers.Models.ViewModels;

namespace CoolShopFlowers.Repos.Abstract
{
    public interface IUserAuthService
    {
        // TODO: Закоментити
        Task<Status> LoginAsync(Login model);
        Task<Status> RegistrAsync(Registr model);
        Task LogoutAsync();
        Task<Status> EditUserAsync(string userId, UserViewModel model);
    }
}
