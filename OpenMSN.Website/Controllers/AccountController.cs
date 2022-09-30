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
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (_authenticationManager.IsAuthenticated)
                return LocalRedirect(returnUrl ?? "/");

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

            if (!user.Activated)
            {
                ModelState.AddModelError("EmailAddress", "Account is currently pending activation. Please check your email inbox to activate.");
                return View(model);
            }

            await _authenticationManager.SignInAsync(user.Id);

            return LocalRedirect(returnUrl ?? "/");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (_authenticationManager.IsAuthenticated)
                return LocalRedirect("/");

            if (!ModelState.IsValid)
                return View(model);

            if (_dbContext.Users.Count() > 0)
            {
                User? existingUser = _dbContext.Users.Where(x => x.EmailAddress == model.EmailAddress).FirstOrDefault();

                if (existingUser is not null)
                {
                    if (existingUser.Activated)
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

            User user = new() { Username = model.EmailAddress, EmailAddress = model.EmailAddress };
            user.SetPassword(model.Password);

            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();

            //TextPart message = new(TextFormat.Html);
            //message.Text = $"Click <a href=\"https://{HttpContext.Request.Host}/Account/Activation?token={user.ActivationToken}\">here</a> to verify.";

            //await user.SendEmail("OpenMSN Account Activation", message);

            return LocalRedirect("/Account/Activation");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // don't use authorize attribute, since if we're not logged in it would just ask us to log in anyway
            if (_authenticationManager.IsAuthenticated)
                await _authenticationManager.SignOutAsync();

            return LocalRedirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> Activation(string? token = null)
        {
            if (_authenticationManager.IsAuthenticated)
                return LocalRedirect("/");

            if (token is not null)
            {
                User? user = _dbContext.Users.Where(x => x.ActivationToken == token && !x.Activated).FirstOrDefault();

                if (user is not null)
                {
                    user.Activated = true;

                    await _dbContext.SaveChangesAsync();

                    await _authenticationManager.SignInAsync(user.Id);
                    return LocalRedirect("/");
                }
            }

            return View();
        }
    }
}
