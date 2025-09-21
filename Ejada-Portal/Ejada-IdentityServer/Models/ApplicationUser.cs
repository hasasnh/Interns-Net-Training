using Microsoft.AspNetCore.Identity;

namespace Ejada_IdentityServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
