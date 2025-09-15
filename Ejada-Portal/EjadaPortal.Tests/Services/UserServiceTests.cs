using System.Text;
using Application.ServiceManager;
using Application.Services;
using Domain.Entities;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;

namespace EjadaPortal.Tests.Services
{
    public class UserServiceTests
    {
        private static UserService CreateSut(Mock<UserManager<User>> userManagerMock, out Mock<IEmailSender> emailSenderMock, out Mock<IEmailTemplateRenderer> templateMock)
        {
            var uow = new Mock<IUnitOfWork>();
            var signInManager = SignInManagerFactory.Create(userManagerMock);

            emailSenderMock = new Mock<IEmailSender>(MockBehavior.Strict);
            templateMock = new Mock<IEmailTemplateRenderer>(MockBehavior.Strict);

            return new UserService(
                uow.Object,
                userManagerMock.Object,
                signInManager,
                emailSenderMock.Object,
                templateMock.Object
            );
        }
       
        [Fact]
        public async Task SendPasswordResetLinkAsync_ReturnsFalse_WhenEmailNotFound()
        {
            // Arrange
            var userManager = UserManagerMockHelper.Create();
            userManager.Setup(m => m.FindByEmailAsync("no@x.com"))
                       .ReturnsAsync((User)null!);

            var sut = CreateSut(userManager, out var emailSender, out var template);

            // Act
            var ok = await sut.SendPasswordResetLinkAsync("no@x.com", "http://base/reset");

            // Assert
            ok.Should().BeFalse();
            emailSender.Verify(m => m.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            template.Verify(m => m.RenderAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), default), Times.Never);
        }

        [Fact]
        public async Task SendPasswordResetLinkAsync_SendsMail_WithCorrectTokens_AndSubject()
        {
            // Arrange
            var user = new User { Email = "u@x.com", UserName = "u1", Name = "Mohammad" };
            var token = "RESET_TOKEN";
            var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var baseUrl = "http://app/reset";

            var userManager = UserManagerMockHelper.Create();
            userManager.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            userManager.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(token);

            var sut = CreateSut(userManager, out var emailSender, out var template);

            IDictionary<string, string>? capturedTokens = null;
            template.Setup(t => t.RenderAsync("ResetPassword.html",
                    It.IsAny<IDictionary<string, string>>(), default))
                .Callback<string, IDictionary<string, string>, CancellationToken>((_, tok, __) => capturedTokens = tok)
                .ReturnsAsync("<html>ok</html>");

            emailSender.Setup(e => e.SendAsync(user.Email,
                                               It.Is<string>(s => s.Contains("إعادة تعيين كلمة المرور")),
                                               "<html>ok</html>"))
                       .Returns(Task.CompletedTask);

            var expectedResetLink = $"{baseUrl}?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(tokenEnc)}";

            // Act
            var ok = await sut.SendPasswordResetLinkAsync(user.Email, baseUrl);

            // Assert
            ok.Should().BeTrue();
            template.Verify(t => t.RenderAsync("ResetPassword.html", It.IsAny<IDictionary<string, string>>(), default), Times.Once);
            emailSender.Verify(e => e.SendAsync(user.Email, It.IsAny<string>(), "<html>ok</html>"), Times.Once);

            capturedTokens.Should().NotBeNull();
            capturedTokens!["DisplayName"].Should().Contain("Mohammad");
            capturedTokens!["Email"].Should().Be("u@x.com");
            capturedTokens!["ResetLink"].Should().Be(expectedResetLink);
            capturedTokens!["RequestTime"].Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ResetPasswordAsync_ReturnsError_WhenUserNotFound()
        {
            // Arrange

            var userManager = UserManagerMockHelper.Create();
            userManager.Setup(m => m.FindByEmailAsync("no@x.com")).ReturnsAsync((User)null!);

            var sut = CreateSut(userManager, out var emailSender, out var template);
            // Act

            var result = await sut.ResetPasswordAsync("no@x.com", "abc", "P@ssw0rd!");
            // Assert

            result.Succeeded.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Description.Contains("المستخدم غير موجود"));
        }

        [Fact]
        public async Task ResetPasswordAsync_CallsUserManager_WithDecodedToken()
        {
            // Arrange

            var user = new User { Email = "u@x.com" };
            var originalToken = "RESET_TOKEN_123";
            var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(originalToken));

            var userManager = UserManagerMockHelper.Create();
            userManager.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            userManager.Setup(m => m.ResetPasswordAsync(user, originalToken, "NewP@ss!"))
                       .ReturnsAsync(IdentityResult.Success)
                       .Verifiable();

            var sut = CreateSut(userManager, out var emailSender, out var template);
            // Act

            var result = await sut.ResetPasswordAsync(user.Email, tokenEnc, "NewP@ss!");
            // Assert

            result.Succeeded.Should().BeTrue();
            userManager.Verify();  
        }
    }
}