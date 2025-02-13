using Jira.Areas.Identity.Data;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Jira.Controllers
{
    public class TicketController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly SignInManager<JiraUser> _signInManager;
        private readonly UserManager<JiraUser> _userManager;
        private readonly JiraContext _context;

        public TicketController(ILogger<HomeController> logger, SignInManager<JiraUser> signInManager, UserManager<JiraUser> userManager, JiraContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task<ActionResult> CreateNewTicket(string SelectedUser, string Description, string Title, int ProjectId, int SprintId)
        {
            var user = await _userManager.GetUserAsync(User);
            var assignee = await _context.Users.FirstOrDefaultAsync(u => u.Id == SelectedUser);
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == ProjectId);
            var sprint = await _context.Sprints.Include(s => s.Tickets)
                                               .FirstOrDefaultAsync(s => s.Id == SprintId);

            if (sprint.SprintMasterId != user.Id)
            {
                TempData["Error"] = "You don't have rights to create a ticket";
                return View();
            }

            if (project == null || sprint == null)
            {
                return NotFound();
            }

            var ticket = new Ticket
            {
                Assignee = assignee,
                AssigneeId = SelectedUser,
                Description = Description,
                ProjectId = ProjectId,
                Project = project,
                Title = Title,
                Sprint = sprint,
                SprintId = SprintId
            };

            _context.Tickets.Add(ticket);
            sprint.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProjectDetailsPage", "Project", new { projectId = ProjectId });
        }
        [HttpPost]
        public async Task<IActionResult> ChangeTicketStatus([FromBody] TicketStatusUpdateRequest request)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(p => p.Id == request.TicketId);
            if (ticket == null)
            {
                return NotFound();
            }
            else if (ticket.Status.Equals(request.NewStatus)) {
                return Json(new { success = true });
            }

           

            switch (request.NewStatus)
            {
                case "ToDo":
                    ticket.Status = TicketStatus.ToDo;
                    break;
                case "InProgress":
                    ticket.Status = TicketStatus.InProgress;
                    break;
                case "Done":
                    ticket.Status = TicketStatus.Done;
                    break;
                default:
                    return BadRequest("Invalid status");
            }

            Console.WriteLine("Nowy stan ticketu: " + ticket.Status);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }


        public class TicketStatusUpdateRequest
        {
            public int TicketId { get; set; }
            public string NewStatus { get; set; }
        }




        public async Task<ActionResult> ChangeTicketPriority(int ticketId, string newStatus)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(p => p.Id == ticketId);

            switch (newStatus)
            {
                case "Critical":
                    ticket.Priority = TicketPriority.Critical;
                    break;
                case "Low":
                    ticket.Priority = TicketPriority.Low;
                    break;
                case "High":
                    ticket.Priority = TicketPriority.High;
                    break;

            }

            return View();
        }

        public async Task<ActionResult> GetAllUserTickets()
        {
            var user = await _userManager.GetUserAsync(User);
        
            var tickets = await _context.Tickets.Where(s => s.AssigneeId==user.Id).ToListAsync();



            return View(tickets);
        }
    }
}
