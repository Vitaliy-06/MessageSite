using MessageSiteAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;

namespace MessageSiteAPI.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<IdentityUser> userManager, IEmailService emailService,
            ILogger<AuthService> logger, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IdentityUser?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            return user;
        }

        public async Task<IdentityResult> RegisterAsync(string email, string password)
        {
            var newUser = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(newUser, password);

            if (!result.Succeeded)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var isSent = await _emailService.SendConfirmationEmailAsync(email, CreateConfirmationLink(newUser, token));

            if (!isSent)
            {
                await _userManager.DeleteAsync(newUser);
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Failed to send confirmation email."
                });
            }

            return IdentityResult.Success;
        }


        public async Task<SignInResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (!result)
            {
                return SignInResult.Failed;
            }
            else if (!user.EmailConfirmed)
            {
                return SignInResult.NotAllowed;
            }

            return SignInResult.Success;
        }

        public Task LogoutAsync()
        {
            return null;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found."
                });

            var decodedToken = HttpUtility.UrlDecode(token).Replace(' ', '+');

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return IdentityResult.Failed(new IdentityError
                {
                    Description = string.Join(", ", result.Errors.Select(e => e.Description))
                });


            return IdentityResult.Success;
        }

        public async Task<bool> IsEmailConfirmedAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            return user.EmailConfirmed;
        }

        public async Task<bool> ResendEmailConfirmationAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return false;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var isSent = await _emailService.SendConfirmationEmailAsync(email, CreateConfirmationLink(user, token));
            if (!isSent) return false;

            return true;
        }

        public string GenerateJwtToken(IdentityUser user)
        {
            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, "User")
            };

            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_config["Jwt:Key"]));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                signingCredentials: signingCredentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }

        private string CreateConfirmationLink(IdentityUser user, string token)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                throw new Exception("No active HTTP request context");

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/api/auth/confirm-email?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";
        }

    }
}
