using Application.DTOs;
using Application.ServiceManager;
using Application.Services;
using Application.Services.IServices;
using Domain.Entities;
using Ejada_Portal.Models;
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
        [AllowAnonymous]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration(UserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                var objUser = _serviceManager.UserService.CheckUser(userDTO.Email, userDTO.Username);

                if (objUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Username or Email already exists");

                    return View();
                }
                objUser = _serviceManager.UserService.MapToUser(userDTO);
                var Result = await _serviceManager.UserService.Register(objUser, userDTO.Password);
                if (Result.Succeeded)
                {
                    await _serviceManager.UserService.Login(objUser);
                    TempData["success"] = "Account Created Successfully";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in Result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _serviceManager.UserService.SignOut();
            TempData["success"] = "Logged out Successfully";
            return RedirectToAction("Login");
        }


        // Rest/Forget password Action's -- Mohammad mustafa --

        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var baseUrl = Url.Action(nameof(ResetPassword), "User", null, Request.Scheme)!;

            try
            {
                var sent = await _serviceManager.UserService.SendPasswordResetLinkAsync(model.Email, baseUrl);

                if (!sent)
                {
                    ModelState.AddModelError(nameof(model.Email), "The email you entered does not exist in the system.");
                    return View(model);
                }

                TempData["MailSent"] = "A Password reset link has been sent to your email.";
                return RedirectToAction(nameof(ForgotPassword));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "تعذَّر إرسال البريد. تأكّد من تشغيل SMTP محليًا (smtp4dev) أو استخدم Pickup Folder.");
                return View(model);
            }
        }


        [HttpGet, AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return RedirectToAction(nameof(ForgotPassword));

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(model.ConfirmPassword), "The passwords you entered do not match. Please try again.");
                return View(model);
            }

            var result = await _serviceManager.UserService.ResetPasswordAsync(model.Email, model.Token, model.Password);
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
