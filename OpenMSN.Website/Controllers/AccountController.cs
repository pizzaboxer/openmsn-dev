using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenMSN.Website.Models;
using OpenMSN.Website.Services;
using OpenMSN.Data;
using OpenMSN.Data.Entities;

namespace OpenMSN.Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthenticationManager _authenticationManager;

        public AccountController(ApplicationDbContext dbContext, AuthenticationManager authenticationManager)
        {
            _dbContext = dbContext;
            _authenticationManager = authenticationManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            if (_dbContext.Users.Count() <= 0)
            {
                ModelState.AddModelError("EmailAddress", "Email address/password is invalid.");
                return View(model);
            }

            User? user = _dbContext.Users.Where(x => x.EmailAddress == model.EmailAddress).First();

            if (user is null || !user.VerifyPassword(model.Password))
            {
                ModelState.AddModelError("EmailAddress", "Email address/password is invalid.");
                return View(model);
            }

            await _authenticationManager.SignInAsync(user.Id);

            return LocalRedirect(returnUrl ?? "/");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_dbContext.Users.Count() > 0)
            {
                User? existingUser = _dbContext.Users.Where(x => x.EmailAddress == model.EmailAddress).First();

                if (existingUser is not null)
                {
                    if (existingUser.EmailAddressVerified)
                    {
                        ModelState.AddModelError("EmailAddress", "An account already exists with this email address.");
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("EmailAddress", "An unverified account already exists with this email address. If you don't remember registering an account, you can request a password reset.");
                        return View(model);
                    }
                }
            }

            User user = new() { EmailAddress = model.EmailAddress };
            user.SetPassword(model.Password);

            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();

            //await _authenticationManager.SignInAsync(user.Id);

            //return LocalRedirect("/");

            return LocalRedirect("/Account/VerifyEmail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authenticationManager.SignOutAsync();
            return LocalRedirect("/");
        }
    }
}
