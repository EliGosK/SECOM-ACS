using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SECOM.ACS.Identity;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using System.Collections.Generic;
using System;
using Vereyon.Web;
using SECOM.ACS.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    public class AccountController : AppControllerBase
    {
        [ApplicationAuthorize]
        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            return View(user.ToViewModel());
        }

        // GET: /account/login
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /account/login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel viewModel, string returnUrl)
        {
            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
                return View(viewModel);

            // Verify if a user exists with the provided identity information           
            var user = UserManager.IgnoreCheckPassword ? UserManager.FindByName(viewModel.Username) : await UserManager.FindAsync(viewModel.Username, viewModel.Password);
            // If a user was found
            if (user != null)
            {
                // Then create an identity for it and sign it in
                await SignInAsync(user, viewModel.RememberMe);

                // Update Last Signed In
                user.LastSignedInDate = DateTime.Now;
                await UserManager.UpdateAsync(user);
                

#if DEBUG
                // If the user came from a specific page, redirect back to it
                return RedirectToLocal(returnUrl);
#else
            if (user.LastChangePassword.HasValue)
               return RedirectToLocal(returnUrl);
            else
               return RedirectToAction("ChangePassword", "Account");
#endif

            }

            // No existing user was found that matched the given criteria
            FlashMessage.Danger("Invalid username or password.");

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (Request.IsAuthenticated)
                Logout();
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            // Clear any lingering authencation data
            AuthenticationManager.SignOut();

            // Create a claims based identity for the current user
            var identity = await user.GenerateUserIdentityAsync(UserManager);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                FlashMessage.Danger(MessageHelper.InvalidModelState());
                return View(model);
            }

            var user = UserManager.FindByName(User.Identity.Name);
            var result = await UserManager.ChangePasswordAsync(user.Id, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                await SignInAsync(user, false);
                return RedirectToAction("ChangePassword");
            }
            else
            {
                result.Errors.ForEach((string message) => FlashMessage.Danger(message));
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl = "")
        {
            if (!returnUrl.IsNullOrWhiteSpace() && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Dashboard", "Request");
        }

    }
}