namespace Jira.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty; // Ścieżka do pliku na serwerze / chmurze
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }

}
