using Jira.Areas.Identity.Data;

namespace Jira.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.ToDo;
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? SprintId { get; set; }  // Klucz obcy do Sprint
        public Sprint? Sprint { get; set; } // Nawigacja do Sprintu

        public string? AssigneeId { get; set; }
        public JiraUser? Assignee { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Attachment> Attachments { get; set; }
    }


}
