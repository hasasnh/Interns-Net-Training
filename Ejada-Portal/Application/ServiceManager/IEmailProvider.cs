namespace Application.ServiceManager
{
    public interface IEmailProvider
    {
        string Name { get; } //provider name
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }
}
