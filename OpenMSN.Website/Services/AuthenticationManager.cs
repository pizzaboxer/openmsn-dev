using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using OpenMSN.Data;
using OpenMSN.Data.Entities;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;

namespace OpenMSN.Website.Services
{
    public class AuthenticationManager
    {
        // hopefully this isn't too much of an issue?
        // we're only calling HttpContextAccessor from a scoped context
        // (only called once per request), so it's as minimum as possible
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;

        private readonly HttpContext HttpContext;
        private readonly IIdentity CurrentIdentity;

        public bool IsAuthenticated { get; private set; } = false;
        public User CurrentUser { get; private set; }

        public AuthenticationManager(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;

            if (_httpContextAccessor.HttpContext is null)
                throw new Exception("HttpContext is null");

            HttpContext = _httpContextAccessor.HttpContext;

            if (HttpContext.User.Identity is null)
                throw new Exception("Identity is null");

            CurrentIdentity = HttpContext.User.Identity;

            if (CurrentIdentity.IsAuthenticated)
            {
                if (CurrentIdentity.Name is null)
                    throw new Exception("Identity.Name is null when authenticated");

                User? currentUser = _dbContext.Users.Find(Int32.Parse(CurrentIdentity.Name));

                if (currentUser is null)
                {
                    // account was likely removed from the database
                    SignOutAsync().Wait();
                    return;
                }

                CurrentUser = currentUser;
                IsAuthenticated = true;
            }
        }

        public async Task SignInAsync(int userId)
        {
            // we're just using the name claim here since it's the most painless to access lol
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, userId.ToString()),
            };

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new(identity);

            AuthenticationProperties authProperties = new();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        }

        public async Task SignOutAsync()
        {
            await HttpContext.SignOutAsync();
        }
    }
}
