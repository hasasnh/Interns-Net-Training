using Application.DTOs;
using Application.ServiceManager;
using Application.Services.IServices;
using Ejada_Portal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ejada_Portal.Controllers
{
    public class UserController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public UserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // ---------------- Registration/Login ----------------
        [AllowAnonymous]
        public IActionResult Registration() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration(UserDTO userDTO)
        {
            if (!ModelState.IsValid) return View();

            var objUser = _serviceManager.UserService.CheckUser(userDTO.Email, userDTO.Username);
            if (objUser != null)
            {
                ModelState.AddModelError(string.Empty, "Username or Email already exists");
                return View();
            }

            objUser = _serviceManager.UserService.MapToUser(userDTO);
            var result = await _serviceManager.UserService.Register(objUser, userDTO.Password);

            if (result.Succeeded)
            {
                await _serviceManager.UserService.Login(objUser);
                TempData["success"] = "Account Created Successfully";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View();
        }

        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserDTO userDTO)
        {
            if (!ModelState.IsValid) return View();

            var objUser = _serviceManager.UserService.CheckUser(userDTO.Email, userDTO.Username);
            if (objUser == null)
            {
                ModelState.AddModelError(string.Empty, "Username or Email does not exists");
                TempData["error"] = "Username or Email does not exists";
                return View();
            }

            var result = await _serviceManager.UserService.CheckPassword(objUser.UserName, userDTO.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid Password");
                TempData["error"] = "Invalid Password";
                return View();
            }

            await _serviceManager.UserService.Login(objUser);
            TempData["success"] = "Logged in Successfully";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _serviceManager.UserService.SignOut();
            TempData["success"] = "Logged out Successfully";
            return RedirectToAction("Registration");
        }

        // ---------------- Forgot Password ----------------
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string? provider)
        {
            var user = User.Identity?.IsAuthenticated == true
                ? await _serviceManager.UserService.GetCurrentUserAsync(User)
                : null;

            var model = new ForgotPasswordViewModel
            {
                SelectedProvider = provider
                                  ?? user?.PreferredEmailProvider
                                  ?? "Gmail"
            };

            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var baseUrl = Url.Action(nameof(ResetPassword), "User", null, Request.Scheme)!;

            string providerName;

            if (User.Identity?.IsAuthenticated == true)
            {
                if (string.IsNullOrEmpty(model.SelectedProvider))
                {
                    providerName = "Gmail";
                }
                else
                {
                    providerName = model.SelectedProvider;
                }
            }
            else
            {
                providerName = "Gmail";
            }


            try
            {
                var sent = await _serviceManager.UserService.SendPasswordResetLinkAsync( model.Email, baseUrl, providerName );

                if (!sent)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email not found.");
                    return View(model);
                }

                TempData["MailSent"] = $"A Password reset link has been sent via {providerName}.";
                return RedirectToAction(nameof(ForgotPassword));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Failed to send email via {providerName}. {ex.Message}";
                return View(model);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SelectProvider()
        {
            var user = await _serviceManager.UserService.GetCurrentUserAsync(User);

            var model = new SelectProviderViewModel
            {
                AvailableProviders = new List<string> { "Gmail", "Rnwood", "Hotmail" },
                SelectedProvider = user?.PreferredEmailProvider ?? "Gmail" //If the user has a provider saved in the DB , it displays it as the default
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SelectProvider(SelectProviderViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _serviceManager.UserService.GetCurrentUserAsync(User);
            if (user != null)
            {
                user.PreferredEmailProvider = model.SelectedProvider;
                await _serviceManager.UserService.UpdateUserAsync(user);
            }

            return RedirectToAction("ForgotPassword", "User", new { provider = model.SelectedProvider });
        }


        // ---------------- Reset Password ----------------
        [HttpGet, AllowAnonymous]
        public IActionResult ResetPassword(string email, string token, string provider)
        {
            var model = new ResetPasswordViewModel
            {
                Email = email ?? "",
                Token = token ?? "",
                SelectedProvider = string.IsNullOrEmpty(provider) ? "Gmail" : provider
            };
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid) return View(model);

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(model.ConfirmPassword), "Passwords do not match");
                return View(model);
            }

            var result = await _serviceManager.UserService.ResetPasswordAsync(
                model.Email, model.Token, model.Password, model.SelectedProvider
            );

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                return View(model);
            }

            TempData["PasswordChanged"] = "Your password has been changed successfully.";
            return RedirectToAction(nameof(ForgotPassword));
        }
    }
}
