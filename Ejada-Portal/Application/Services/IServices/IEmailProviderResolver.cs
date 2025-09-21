namespace Application.Services.IServices
{
    public interface IEmailProviderResolver
    {
        IEmailProvider Get(string providerName);
    }
}
