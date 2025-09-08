namespace Application.ServiceManager
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(string templateName, IDictionary<string, string> tokens, CancellationToken ct = default);
    }
}
