using Jira.Areas.Identity.Data;

namespace Jira.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public string UserId { get; set; }
        public JiraUser User { get; set; }
    }

}
