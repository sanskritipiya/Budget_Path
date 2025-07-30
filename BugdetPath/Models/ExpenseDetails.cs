namespace BugdetPath.Models;

public class ExpenseDetails
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public int UserId { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime Date { get; set; }
    
    public string Tag { get; set; }
    
    public string CustomTag { get; set; }
    
    public string Notes { get; set; }


}