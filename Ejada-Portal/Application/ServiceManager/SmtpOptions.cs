namespace Application.ServiceManager
{
    public class SmtpOptions
    {
        // Gmail
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? User { get; set; }
        public string? Pass { get; set; }
        public string? From { get; set; }
        public string? FromDisplayName { get; set; }
    }

}
