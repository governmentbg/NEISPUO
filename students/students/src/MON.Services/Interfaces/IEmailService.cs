namespace MON.Services.Interfaces
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailAsync(string email, string subject, string message, byte[] imageData, string imageName);
        Task SendEmailAsync(MailMessage message);
    }
}
