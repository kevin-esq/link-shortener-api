namespace LinkShortener.Application.Abstractions.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body, bool isHtml = false);
    }
}
