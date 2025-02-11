﻿using Jira.Areas.Identity.Data;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> CreateNewTicket(string userId, string description, string title, int projectId, int sprintId)
        {
            var user = await _userManager.GetUserAsync(User);



            var assignee = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            var sprint = await _context.Sprints.Include(s => s.Tickets) // Ważne: Załaduj listę Tickets
                                               .FirstOrDefaultAsync(s => s.Id == sprintId);

            if(sprint.SprintMasterId != user.Id)
            {
                TempData["Error"] = "You dont have rights to create a ticket";
                return View();

            }

            if (project == null || sprint == null)
            {
                return NotFound(); // Obsługa błędów, jeśli projekt lub sprint nie istnieje
            }

            var ticket = new Ticket
            {
                Assignee = assignee,
                AssigneeId = userId,
                Description = description,
                ProjectId = projectId,
                Project = project,
                Title = title,
                Sprint = sprint,
                SprintId = sprintId
            };

            _context.Tickets.Add(ticket);

            // Ręczne dodanie do kolekcji Sprint.Tickets
            sprint.Tickets.Add(ticket);

            await _context.SaveChangesAsync();
            return View();
        }


        public async Task<ActionResult> ChangeTicketStatus(int ticketId, string newStatus)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(p=> p.Id == ticketId);
       
            switch (newStatus)
                {
                case "InProgress":
                    ticket.Status=TicketStatus.InProgress;
                    break;
                case "InReview":
                    ticket.Status =TicketStatus.InReview;
                    break;
                case "Done":
                    ticket.Status = TicketStatus.Done;
                    break;
                
                }

            return View();
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
