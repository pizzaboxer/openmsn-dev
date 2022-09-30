using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenMSN.Data;
using OpenMSN.Website.Models;
using OpenMSN.Website.Services;
using System.Diagnostics;

namespace OpenMSN.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(AuthenticationManager authenticationManager, ApplicationDbContext dbContext)
        {
            _authenticationManager = authenticationManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}