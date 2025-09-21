using Application.DTOs;
using Application.ServiceManager;
using Application.Services.IServices;
using Microsoft.AspNetCore.Authentication;
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
        // ************** to route to IdentityServer registration page **************

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ************** to route to IdentityServer registration page **************


        //[AllowAnonymous]
        //public IActionResult Registration()
        //{
        //    return View();
        //}
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

        // ************** to route to IdentityServer login page **************

        [HttpGet]
     [Authorize]
        public async Task<IActionResult> Login()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index), "Home");

        }

        // ************** to route to IdentityServer login page **************


        //[AllowAnonymous]
        //public IActionResult Login()
        //{
        //    return View();
        //}

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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LogOut()
        //{
        //    await _serviceManager.UserService.SignOut();
        //    TempData["success"] = "Logged out Successfully";
        //    return RedirectToAction("Registration");
        //}
    }
}
