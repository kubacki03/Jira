namespace Jira.Models
{
    public class ChangeStatusRequest
    {
        public int TicketId { get; set; }
        public string NewStatus { get; set; }
    }
}
