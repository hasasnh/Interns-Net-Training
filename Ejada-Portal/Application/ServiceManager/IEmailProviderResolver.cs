namespace Application.ServiceManager
{
    public interface IEmailProviderResolver
    {
        IEmailProvider Get(string providerName);
    }
}
