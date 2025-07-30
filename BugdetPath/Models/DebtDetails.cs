namespace BugdetPath.Models
{
    public class DebtDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }

        public string Source { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
        

        public DateTime DueDate { get; set; }
        public bool IsCleared { get; set; }
        public int UserId { get; set; }
    }
}

