using Application.Services.IServices;


namespace Application.ServiceManager
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }
}
