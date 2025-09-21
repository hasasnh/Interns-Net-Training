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
using Microsoft.EntityFrameworkCore.Metadata;

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
            out Mock<IEmailProvider> rnwood)
        {
            uow = new Mock<IUnitOfWork>();
            signIn = SignInManagerFactory.Create(um);

            resolver = new Mock<IEmailProviderResolver>(MockBehavior.Strict);
            template = new Mock<IEmailTemplateRenderer>(MockBehavior.Strict);

            gmail = new Mock<IEmailProvider>(MockBehavior.Strict);
            rnwood = new Mock<IEmailProvider>(MockBehavior.Strict);

            gmail.SetupGet(p => p.Name).Returns("Gmail");
            rnwood.SetupGet(p => p.Name).Returns("Rnwood");

            resolver.Setup(r => r.Get(It.Is<string>(s => s != null && s.Equals("Gmail", System.StringComparison.OrdinalIgnoreCase))))
                    .Returns(gmail.Object);
            resolver.Setup(r => r.Get(It.Is<string>(s => s != null && s.Equals("Rnwood", System.StringComparison.OrdinalIgnoreCase))))
                    .Returns(rnwood.Object);

            return new UserService(uow.Object, um.Object, signIn, resolver.Object, template.Object);
        }

        // Positive: Successfully sent the reset link via Gmail
         [Fact]
        public async Task SendPasswordResetLink_Gmail_Success_SendsMail_WithTokenAndEmailInLink()
        {
            // Arrange
            var user = new User { Email = "u@x.com", UserName = "u1", Name = "Mohammad" };
            var token = "RESET_TOKEN";
            var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var baseUrl = "http://app/User/ResetPassword";

            var um = UserManagerMockHelper.Create();
            um.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            um.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(token);

            var sut = CreateSut(um, out var uow, out var signIn, out var resolver, out var template, out var gmail, out var rnwood);

            IDictionary<string, string>? capturedTokens = null;
            template.Setup(t => t.RenderAsync("ResetPassword.html", It.IsAny<IDictionary<string, string>>(), default))
                    .Callback<string, IDictionary<string, string>, System.Threading.CancellationToken>((_, tok, __) => capturedTokens = new Dictionary<string, string>(tok))
                    .ReturnsAsync("<html>ok</html>");

            gmail.Setup(p => p.SendAsync(user.Email,
                                         It.Is<string>(s => s.Contains("Reset Password -")),
                                         "<html>ok</html>"))
                 .Returns(Task.CompletedTask)
                 .Verifiable();

            // Act
            var ok = await sut.SendPasswordResetLinkAsync(user.Email, baseUrl, "Gmail");

            // Assert
            ok.Should().BeTrue();
            gmail.Verify();
            capturedTokens.Should().NotBeNull();
            capturedTokens!["Email"].Should().Be(user.Email);
            capturedTokens!["ResetLink"].Should().Contain($"email={Uri.EscapeDataString(user.Email)}");
            capturedTokens!["ResetLink"].Should().Contain($"token={Uri.EscapeDataString(tokenEnc)}");
        }


        // Negative: Email does not exist → False and no sending
        [Fact]
        public async Task SendPasswordResetLink_ReturnsFalse_WhenUserNotFound()
        {
            // Arrange
            var um = UserManagerMockHelper.Create();
            um.Setup(m => m.FindByEmailAsync("no@x.com")).ReturnsAsync((User)null!);

            var sut = CreateSut(um, out var uow, out var signIn, out var resolver, out var template, out var gmail, out var rnwood);

            // Act
            var ok = await sut.SendPasswordResetLinkAsync("no@x.com", "http://app/User/ResetPassword", "Gmail");

            // Assert
            ok.Should().BeFalse();
            template.Verify(t => t.RenderAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), default), Times.Never);
            gmail.Verify(p => p.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        // Negative: Provider not registered → Exception
        [Fact]
        public async Task SendPasswordResetLink_Throws_WhenProviderNotRegistered()
        {
            // Arrange
            var user = new User { Email = "u@x.com" };
            var um = UserManagerMockHelper.Create();
            um.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);

            var uow = new Mock<IUnitOfWork>();
            var signIn = SignInManagerFactory.Create(um);

            var resolver = new Mock<IEmailProviderResolver>(MockBehavior.Strict);
            resolver.Setup(r => r.Get(It.IsAny<string>())).Returns((IEmailProvider)null!);

            var template = new Mock<IEmailTemplateRenderer>(MockBehavior.Strict);

            var sut = new UserService(uow.Object, um.Object, signIn, resolver.Object, template.Object);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await sut.SendPasswordResetLinkAsync(user.Email, "http://app/User/ResetPassword", "Gmail")
            );
        }



    }
}
