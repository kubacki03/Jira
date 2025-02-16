using Jira.Areas.Identity.Data;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Jira.Controllers
{
    public class CommentController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        public readonly SignInManager<JiraUser> _signInManager;
        private readonly UserManager<JiraUser> _userManager;
        private readonly JiraContext _context;

        public CommentController(ILogger<HomeController> logger, SignInManager<JiraUser> signInManager, UserManager<JiraUser> userManager, JiraContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> AddComment(string content, int ticketId)
        {
            var user =await _userManager.GetUserAsync(User);
            var ticket= _context.Tickets.Include(s=> s.Comments).FirstOrDefault(t => t.Id == ticketId);

            Comment comment = new Comment { Content = content, TicketId = ticketId, User=user,UserId=user.Id, Ticket=ticket };
            _context.Comments.Add(comment);
            ticket.Comments.Add(comment);

            _context.SaveChanges();

            return RedirectToAction("GetTicketDetails", "Ticket", new { ticketId = ticketId });
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file,int ticketId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nie wybrano pliku.");
            }

            long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
            {
                return BadRequest("Plik jest za duży! Maksymalny rozmiar to 5 MB.");
            }

            // Ścieżka zapisu pliku
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadPath); // Tworzy folder, jeśli nie istnieje

            // Unikalna nazwa pliku
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            // Zapis pliku na serwerze
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);
            Attachment attachment = new Attachment { FileName = fileName, FilePath = filePath, Ticket = ticket, TicketId = ticket.Id };  
            _context.Attachments.Add(attachment);
            ticket.Attachments.Add(attachment);
            _context.SaveChanges();

            return Ok($"Plik zapisany: {fileName}");
        }

    }
}
