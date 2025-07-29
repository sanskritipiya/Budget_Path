using BugdetPath.Models;

namespace BugdetPath.Services;

public interface IUserService
{
        Task<bool> RegisterUserAsync(User user);
        
        Task<List<User>> LoadUsersAsync();
        
        Task SaveUserAsync(User user);
        
        Task<User?> LoginAsync(string username, string password);
                

    

        
   
}