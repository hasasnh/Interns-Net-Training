using Application.Services.IServices;

namespace Application.Services
{
    public class EmailProviderResolver : IEmailProviderResolver
    {
        private readonly GmailEmailProvider _gmail;
        private readonly RnwoodEmailProvider _rnwood;

        public EmailProviderResolver(GmailEmailProvider gmail, RnwoodEmailProvider rnwood)
        {
            _gmail = gmail ?? throw new ArgumentNullException(nameof(gmail));
            _rnwood = rnwood ?? throw new ArgumentNullException(nameof(rnwood));
        }

        public IEmailProvider Get(string providerName)
        {
            if (string.Equals(providerName, "Gmail", StringComparison.OrdinalIgnoreCase))
                return _gmail;

            if (string.Equals(providerName, "Rnwood", StringComparison.OrdinalIgnoreCase))
                return _rnwood;

            return _gmail; // by Default choose Gmail
        }
    }
}
