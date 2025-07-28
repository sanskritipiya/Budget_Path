using BugdetPath.Models;

namespace BugdetPath.Services;

public interface IUserService
{
    Task SaveUserAsync(UserDetail user);
        
    Task<List<UserDetail>> LoadUsersAsync();
}