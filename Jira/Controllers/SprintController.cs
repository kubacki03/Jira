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

        [HttpPost]
        public async Task<IActionResult> CreateNewSprint(Sprint sprint, int projectId)
        {
            var user = await _userManager.GetUserAsync(User);
            var project = await _context.Projects.Include(x => x.Sprints)
                                                 .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound("Projekt nie istnieje.");
            }

            sprint.SprintMaster = user;
            sprint.SprintMasterId = user.Id;
            sprint.Project = project;
            sprint.ProjectId = projectId;

            project.Sprints.Add(sprint);
            _context.Sprints.Add(sprint);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProjectDetailsPage", "Project", new { projectId = projectId });

        }



        public Sprint GetCurrentSprintInProject(int projectId) {

            var sprint = _context.Sprints
     .FirstOrDefault(p => p.ProjectId == projectId && p.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= p.EndDate);

            return sprint ;
        }

        //Get all tickets for all users in sprint
        public IActionResult GetTicketsFromSprint(int projectId)
        {
            var sprint = GetCurrentSprintInProject(projectId);

            if (sprint == null)
            {
                // Możesz zwrócić pusty widok, komunikat błędu lub przekierowanie
                return View(new List<Ticket>());
            }

            var tickets = _context.Tickets.Where(t => t.SprintId == sprint.Id).ToList();
            return View(tickets);
        }


    }
}
