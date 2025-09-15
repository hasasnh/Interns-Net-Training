using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace EjadaPortal.Tests.Helpers
{
    public static class SignInManagerFactory
    {
        public static SignInManager<User> Create(Mock<UserManager<User>> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            var options = Options.Create(new IdentityOptions());
            var logger = new Mock<ILogger<SignInManager<User>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<User>>();

            return new SignInManager<User>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options,
                logger.Object,
                schemes.Object,
                confirmation.Object 
            );
        }
    }
}
