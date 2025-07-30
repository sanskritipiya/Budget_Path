namespace BugdetPath.Models
{
    public class DebtDetails
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public string Source { get; set; } = string.Empty;
        
        public decimal Amount { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public bool IsCleared { get; set; } = false;
        
        public DateTime CreatedAt { get; set; }
    }
}

