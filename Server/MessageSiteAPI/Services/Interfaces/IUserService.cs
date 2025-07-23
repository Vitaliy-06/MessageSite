using Microsoft.AspNetCore.Identity;

namespace MessageSiteAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<IdentityUser>> GetAllUsers();
        Task<IdentityUser?> GetUserByEmail(string email);
    }
}
