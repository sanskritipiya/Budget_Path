namespace BugdetPath.Models;

public class TransactionDetails
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public decimal Amount { get; set; }
    
    public string Notes { get; set; }
    
    public string Tag { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Type { get; set; }

    public int UserId { get; set; }

}