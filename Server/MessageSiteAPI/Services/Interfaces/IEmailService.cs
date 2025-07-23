namespace MessageSiteAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendConfirmationEmailAsync(string email, string token);
    }
}
