namespace BugdetPath.Models;

public class User
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string UserName { get; set; }
    
    public string Password { get; set; }

    public string TotalBalance { get; set; }
    
    public string PreferredCurrency{ get; set; }
}