using Jira.Areas.Identity.Data;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jira.Controllers
{
    public class SprintController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        public readonly SignInManager<JiraUser> _signInManager;
        private readonly UserManager<JiraUser> _userManager;
        private readonly JiraContext _context;

        public SprintController(ILogger<HomeController> logger, SignInManager<JiraUser> signInManager, UserManager<JiraUser> userManager, JiraContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<ActionResult> CreateNewSprint(Sprint sprint, int projectId)
        {
            var user = await _userManager.GetUserAsync(User);
            var project =await _context.Projects.Include(x=> x.Sprints).FirstOrDefaultAsync(p => p.Id == projectId);
            sprint.SprintMaster = user;
            sprint.SprintMasterId = user.Id;
            sprint.Project = project;
            sprint.ProjectId= projectId;
            project.Sprints.Add(sprint);
            _context.Sprints.Add(sprint);
            _context.SaveChanges();

            return View();
        }

    }
}
