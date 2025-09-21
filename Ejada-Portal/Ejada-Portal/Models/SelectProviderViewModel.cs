namespace Ejada_Portal.Models
{
    public class SelectProviderViewModel
    {
        public string SelectedProvider { get; set; } = "Gmail";
        public List<string> AvailableProviders { get; set; } = new List<string> { "Gmail", "Rnwood" };
    }
}
