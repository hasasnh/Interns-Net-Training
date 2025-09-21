using System.Text;
using Application.Services;
using Application.ServiceManager;
using Domain.Entities;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using Xunit;

namespace EjadaPortal.Tests.Tests.Services
{
    public class UserService_Core_Tests
    {
        private static UserService CreateSut(
            Mock<UserManager<User>> um,
            out Mock<IUnitOfWork> uow,
            out SignInManager<User> signIn,
            out Mock<IEmailProviderResolver> resolver,
            out Mock<IEmailTemplateRenderer> template,
            out Mock<IEmailProvider> gmail,
            out Mock<IEmailProvider> rnwood,
            out Mock<IEmailProvider> hotmail)
        {
            // Arrange dependencies
            uow = new Mock<IUnitOfWork>();
            signIn = SignInManagerFactory.Create(um);

            resolver = new Mock<IEmailProviderResolver>(MockBehavior.Strict);
            template = new Mock<IEmailTemplateRenderer>(MockBehavior.Strict);

            gmail = new Mock<IEmailProvider>(MockBehavior.Strict);
            rnwood = new Mock<IEmailProvider>(MockBehavior.Strict);
            hotmail = new Mock<IEmailProvider>(MockBehavior.Strict);

            gmail.SetupGet(p => p.Name).Returns("Gmail");
            rnwood.SetupGet(p => p.Name).Returns("Rnwood");
            hotmail.SetupGet(p => p.Name).Returns("Hotmail");

            resolver.Setup(r => r.Get(It.Is<string>(s => s != null && s.Equals("Gmail", StringComparison.OrdinalIgnoreCase))))
                    .Returns(gmail.Object);
            resolver.Setup(r => r.Get(It.Is<string>(s => s != null && s.Equals("Rnwood", StringComparison.OrdinalIgnoreCase))))
                    .Returns(rnwood.Object);
            resolver.Setup(r => r.Get(It.Is<string>(s => s != null && s.Equals("Hotmail", StringComparison.OrdinalIgnoreCase))))
                    .Returns(hotmail.Object);

            return new UserService(uow.Object, um.Object, signIn, resolver.Object, template.Object);
        }

        // Positive: Successfully sending reset link via Gmail
        [Fact]
        public async Task SendPasswordResetLink_Gmail_Success()
        {
            // Arrange
            var user = new User { Email = "u@x.com", UserName = "u1" };
            var token = "RESET_TOKEN";
            var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var baseUrl = "http://app/User/ResetPassword";

            var um = UserManagerMockHelper.Create();
            um.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            um.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(token);

            var sut = CreateSut(um, out _, out _, out _, out var template, out var gmail, out _, out _);

            template.Setup(t => t.RenderAsync("ResetPassword.html", It.IsAny<IDictionary<string, string>>(), default))
                    .ReturnsAsync("<html>ok</html>");
            gmail.Setup(p => p.SendAsync(user.Email, It.IsAny<string>(), "<html>ok</html>"))
                 .Returns(Task.CompletedTask)
                 .Verifiable();

            // Act
            var ok = await sut.SendPasswordResetLinkAsync(user.Email, baseUrl, "Gmail");

            // Assert
            ok.Should().BeTrue();
            gmail.Verify();
        }

        // Negative: User not found
        [Fact]
        public async Task SendPasswordResetLink_ReturnsFalse_WhenUserNotFound()
        {
            // Arrange
            var um = UserManagerMockHelper.Create();
            um.Setup(m => m.FindByEmailAsync("no@x.com")).ReturnsAsync((User)null!);

            var sut = CreateSut(um, out _, out _, out _, out var template, out var gmail, out _, out _);

            // Act
            var ok = await sut.SendPasswordResetLinkAsync("no@x.com", "http://app/User/ResetPassword", "Gmail");

            // Assert
            ok.Should().BeFalse();
            template.Verify(t => t.RenderAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), default), Times.Never);
            gmail.Verify(p => p.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        // Negative: Provider not registered
        [Fact]
        public async Task SendPasswordResetLink_Throws_WhenProviderNotRegistered()
        {
            // Arrange
            var user = new User { Email = "u@x.com" };
            var um = UserManagerMockHelper.Create();
            um.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            um.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("VALID_TOKEN"); // ensure token is not null

            var uow = new Mock<IUnitOfWork>();
            var signIn = SignInManagerFactory.Create(um);

            var resolver = new Mock<IEmailProviderResolver>(MockBehavior.Strict);
            resolver.Setup(r => r.Get(It.IsAny<string>()))
                    .Throws(new KeyNotFoundException("Provider not found")); //force expected exception

            var template = new Mock<IEmailTemplateRenderer>();
            template.Setup(t => t.RenderAsync("ResetPassword.html", It.IsAny<IDictionary<string, string>>(), default))
                    .ReturnsAsync("<html>ok</html>");

            var sut = new UserService(uow.Object, um.Object, signIn, resolver.Object, template.Object);

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await sut.SendPasswordResetLinkAsync(user.Email, "http://app/User/ResetPassword", "Unknown")
            );
        }

    }
}
