using Jira.Areas.Identity.Data;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Controllers
{
    public class ProjectController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        public readonly SignInManager<JiraUser> _signInManager;
        private readonly UserManager<JiraUser> _userManager;
        private readonly JiraContext _context;

        public ProjectController(ILogger<HomeController> logger, SignInManager<JiraUser> signInManager, UserManager<JiraUser> userManager,JiraContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public async Task<ActionResult> CreateNewProject(string description, string name, string password)
        {
            var user = await _userManager.GetUserAsync(User);
            Project newProject = new Project { CreatedAt = DateTime.Now, Description = description, Name=name, Creator= user, Password=password};

            return View();
        }


        [Authorize]
        public async Task<ActionResult> JoinAProject(long projectId, string password)
        {
            var user = await _userManager.GetUserAsync(User);
            var project = _context.Projects.FirstOrDefault(p => (p.Id == projectId && p.Password.Equals(password)));

            if (project == null) {
                TempData["Error"] = "Password incorrect";
                return View();
            }

            if (project.Users.Contains(user))
            {
                TempData["Error"] = "You already exist in project";
            }

            project.Users.Add(user);
            _context.SaveChanges();

            return View();
        }
    }
}
