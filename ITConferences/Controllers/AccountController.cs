using ITConferences.Managers;
using ITConferences.Models;
using ITConferences.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Resources;

namespace ITConferences.Controllers
{
	public class AccountController : Controller
	{
        private UserProvider _provider = new UserProvider();

        public IActionResult Login()
		{
			return View();
		}

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser? user = _provider.GetUser(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "Login unsuccessfull!");
                return View(model);
            }

            if (user.password != model.Password)
            {
                ModelState.AddModelError("", "Password not correct");
                return View(model);
            }

            if (!string.IsNullOrEmpty(user.userName))
            {
                HttpContext.Session.SetString("UserName", user.userName);
            }

            return RedirectToLocal(model.ReturnUrl);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Confirm password not match!");
                }
                else
                {
                    var user = new ApplicationUser
                    {
                        userName = model.UserName,
                        password = model.Password,
                        role = Enums.Role.User
                    };

                    var success = _provider.AddUser(user, out string errorMessage);

                    if (success)
                    {
                        return View("Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", errorMessage ?? "Register not successfull");
                    }
                }

            }

            return View(model);
        }

        private ActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index", "Home");
        }
    }
}