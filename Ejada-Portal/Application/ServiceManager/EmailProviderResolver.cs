using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.ServiceManager
{
    public class EmailProviderResolver : IEmailProviderResolver
    {
        private readonly Dictionary<string, IEmailProvider> _providers;

        public EmailProviderResolver(IEnumerable<IEmailProvider> providers)
        {
            _providers = providers.ToDictionary(
                p => p.Name,
                StringComparer.OrdinalIgnoreCase
            );
        }

        public IEmailProvider Get(string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                providerName = "Gmail"; // default

            if (_providers.TryGetValue(providerName, out var provider))
                return provider;

            throw new KeyNotFoundException($"Provider '{providerName}' not found. Registered: {string.Join(", ", _providers.Keys)}");
        }
    }

}
