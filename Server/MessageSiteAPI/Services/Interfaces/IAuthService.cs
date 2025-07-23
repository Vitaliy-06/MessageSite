using Microsoft.AspNetCore.Identity;

namespace MessageSiteAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(string email, string password);
        Task<SignInResult> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<IdentityUser> GetUserByEmailAsync(string email);
        
        Task<bool> ResendEmailConfirmationAsync(string email);
        Task<bool> IsEmailConfirmedAsync(string email);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);

        string GenerateJwtToken(IdentityUser user);

    }
}
