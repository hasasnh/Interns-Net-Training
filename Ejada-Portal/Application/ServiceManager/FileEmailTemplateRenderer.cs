using System.Text;
using Microsoft.Extensions.Hosting;              // IHostEnvironment
using Microsoft.Extensions.Logging;

namespace Application.ServiceManager
{
    public class FileEmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<FileEmailTemplateRenderer> _log;

        public FileEmailTemplateRenderer(IHostEnvironment env, ILogger<FileEmailTemplateRenderer> log)
        {
            _env = env;
            _log = log;
        }

        public async Task<string> RenderAsync(string templateName, IDictionary<string, string> tokens, CancellationToken ct = default)
        {
            var root = Path.Combine(_env.ContentRootPath, "EmailTemplates");
            var path = Path.Combine(root, templateName);

            if (!File.Exists(path))
            {
                _log.LogError("Email template not found at {Path}", path);
                throw new FileNotFoundException($"Email template not found: {path}");
            }

            var html = await File.ReadAllTextAsync(path, Encoding.UTF8, ct);

            if (tokens != null)
            {
                foreach (var kv in tokens)
                    html = html.Replace("{{" + kv.Key + "}}", kv.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            return html;
        }
    }
}
