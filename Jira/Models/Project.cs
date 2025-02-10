using System.Net.Sockets;
using Jira.Areas.Identity.Data;

namespace Jira.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Password { get; set; }

        // Klucz obcy do JiraUser
        public string CreatorId { get; set; }
        public JiraUser Creator { get; set; }

        public ICollection<JiraUser> Users { get; set; } = new List<JiraUser>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
    }


}
