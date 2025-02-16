using Jira.Areas.Identity.Data;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> CreateNewSprint(Sprint sprint, int projectId)
        {
            var user = await _userManager.GetUserAsync(User);
            var project = await _context.Projects.Include(x => x.Sprints)
                                                 .FirstOrDefaultAsync(p => p.Id == projectId);

            var existingSprint = project.Sprints.FirstOrDefault(r => (r.StartDate >= sprint.StartDate && r.StartDate <= sprint.EndDate)
                         || (r.EndDate >= sprint.StartDate && r.EndDate <= sprint.EndDate)
                         || (r.StartDate < sprint.StartDate && r.EndDate > sprint.EndDate));

            if (existingSprint != null)
            {
                TempData["Error"] = "Istnieje juz sprint w tym czasie";
                return RedirectToAction("ProjectDetailsPage", "Project", new { projectId = projectId });
            }

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


        [Authorize]
        public Sprint GetCurrentSprintInProject(int projectId) {

            var sprint = _context.Sprints.Include(z=> z.Tickets)
     .FirstOrDefault(p => p.ProjectId == projectId && p.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= p.EndDate);

            return sprint ;
        }

        [Authorize]
        public IActionResult GetTicketsFromSprint(int projectId)
        {
            //jest nullem
            var sprint = GetCurrentSprintInProject(projectId);

            if (sprint == null)
            {
           
                return View(new List<Ticket>());
            }

            var tickets = _context.Tickets.Include(a => a.Assignee).Where(t => t.SprintId == sprint.Id).ToList();
            return View(tickets);
        }


        [Authorize]
        public async Task<IActionResult> GetSprintDetailsPage(int sprintId)
        {
            var sprint = await _context.Sprints
                .Include(s => s.Tickets)
                .Include(s => s.Project) // Dodatkowe załadowanie projektu
                .ThenInclude(p => p.Users) // Załaduj użytkowników projektu
                .FirstOrDefaultAsync(p => p.Id == sprintId);

            if (sprint == null)
            {
                return NotFound("Sprint nie został znaleziony.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Nie można znaleźć zalogowanego użytkownika.");
            }
            var x = sprint.SprintMasterId;
            TempData["userId"] = user.Id;
            return View(sprint);
        }

    }
}
