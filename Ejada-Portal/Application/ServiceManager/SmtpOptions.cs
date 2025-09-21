namespace Application.ServiceManager
{
    public class SmtpOptions
    {
        public string? Host { get; set; }          // smtp.gmail.com
        public int Port { get; set; } = 587;       // 587 = StartTLS, 465 = SSL
        public string? From { get; set; }          // غالبًا نفس الحساب
        public string? FromDisplayName { get; set; } = "Ejada Portal";
        public string? User { get; set; }          // حساب Gmail
        public string? Pass { get; set; }          // App Password (16 حرف)
    }
}
