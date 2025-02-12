using System.Diagnostics;
using Jira.Areas.Identity.Data;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jira.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly SignInManager<JiraUser> _signInManager;
        private readonly UserManager<JiraUser> _userManager;
        private readonly JiraContext _context;

        public HomeController(ILogger<HomeController> logger, SignInManager<JiraUser> signInManager, UserManager<JiraUser> userManager, JiraContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

           var projects= await _context.Projects.Include(p=>p.Users).Where(s => s.Users.Contains(user)).ToListAsync();



            return View(projects);
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
