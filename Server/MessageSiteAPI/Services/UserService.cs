using MessageSiteAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MessageSiteAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<IdentityUser>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync<IdentityUser>();

            return users;
        }

        public async Task<IdentityUser?> GetUserByEmail(string email)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.Email.Equals(email) && user.EmailConfirmed);

            return user;
        }
    }
}
