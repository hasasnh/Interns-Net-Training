using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Ejada_IdentityServer
{
    public static class SD
    {
        public const string Admin = "admin";
        public const string Customer = "customer";


        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("magic", "Magic Server"),
                new ApiScope(name: "read",   displayName: "Read your data."),
                new ApiScope(name: "write",  displayName: "Write your data."),
                new ApiScope(name: "delete", displayName: "Delete your data.")
            };

        public static IEnumerable<Client> Cleints =>
       new List<Client>
            {
                // machine to machine client
                new Client
                {
                    ClientId = "service.client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1", "api2.read_only" }
                },
                // interactive ASP.NET Core Web App
                new Client
                {
                    ClientId = "magic",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = true,
                    AllowedCorsOrigins = { "https://localhost:7002" },  
                    AllowedScopes = { "magic",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        JwtClaimTypes.Role
                    },
                    // where to redirect to after login
                    RedirectUris={ "https://ejada.portal/signin-oidc" },
                    // where to redirect to after logout
                    PostLogoutRedirectUris={"https://ejada.portal/signout-callback-oidc" },
                    AllowOfflineAccess = true   //added for refresh tokens
                }
            };
    }
}
