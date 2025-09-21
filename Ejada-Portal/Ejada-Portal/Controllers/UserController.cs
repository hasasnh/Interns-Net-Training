using Application.DTOs;
using Application.ServiceManager;
using Application.Services;
using Application.Services.IServices;
using Ejada_Portal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Ejada_Portal.Controllers
{
    public class UserController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public UserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        // ************** to route to IdentityServer registration page **************

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ************** to route to IdentityServer registration page **************


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

        // ************** to route to IdentityServer login page **************

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Login()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index), "Home");

        }

        // ************** to route to IdentityServer login page **************

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                var objUser = _serviceManager.UserService.CheckUser(userDTO.Email, userDTO.Username);
                if (objUser != null)
                {
                    var result = await _serviceManager.UserService.CheckPassword(objUser.UserName, userDTO.Password);
                    if (result.Succeeded)
                    {
                        await _serviceManager.UserService.Login(objUser);
                        TempData["success"] = "Logged in Successfully";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Password");
                        TempData["error"] = "Invalid Password";
                        return View();
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Username or Email does not exists");
                    TempData["error"] = "Username or Email does not exists";
                    return View();
                }
            }
            return View();
        }
        // ************** to logout only from session **************
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            SignOut("Cookies", "oidc");
            return RedirectToAction("Index", "Home");
        }
        // ************** to logout only from session **************

        // ---------------- Forgot Password ----------------
        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPassword(string provider)
        {
            var model = new ForgotPasswordViewModel
            {
                SelectedProvider = string.IsNullOrEmpty(provider) ? "Gmail" : provider
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
        public IActionResult SelectProvider()
        {
            var model = new SelectProviderViewModel
            {
                AvailableProviders = new List<string> { "Gmail", "Rnwood" }
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SelectProvider(SelectProviderViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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
        // -- Mohammad mustafa --
    }
}
