using Jira.Areas.Identity.Data;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

       
        public async Task<ActionResult> CreateNewProject(string description, string name, string password)
        {
            var user = await _userManager.GetUserAsync(User);

            var existProject = _context.Projects.FirstOrDefault(p=> (p.Name == name));

            if (existProject != null) {
                TempData["Error"] = "Project with this name already exists";
                return View();
            }

            Project newProject = new Project { CreatedAt = DateTime.Now, Description = description, Name=name, Creator= user, Password=password};
            _context.Projects.Add(newProject);
            _context.SaveChanges();
            return View();
        }


        public async Task<ActionResult> AddUserToProject(string userId, int projectId)
        {
            var project =await  _context.Projects.Include(p=> p.Users).FirstOrDefaultAsync(p=> p.Id==projectId);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (project.Users.Contains(user))
            {
                TempData["Error"] = "User already existst in project";
            }
            else
            {
                project.Users.Add(user);
                _context.SaveChanges();
            }
            return View();

        }

      
        public async Task<ActionResult> JoinAProject(long projectId, string password)
        {
            var user = await _userManager.GetUserAsync(User);
            var project =await _context.Projects.Include(s=> s.Users).FirstOrDefaultAsync(p => (p.Id == projectId && p.Password.Equals(password)));

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
