namespace BugdetPath.Models;

public class IncomeDetails
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Notes { get; set; }
    
    public DateTime Date { get; set; }
    
    public int UserId { get; set; }
    
    public string Tag { get; set; }
    
    public decimal Amount { get; set; }
}