using BugdetPath.Models;

namespace BugdetPath.Services;

public class AuthenticationService
{
    private const string StorageKey = "authenticated_user";
    private User authenticatedUser;

    public async Task<User> GetAuthenticatedUserAsync()
    {
        return authenticatedUser;
    }

    public void SetAuthenticatedUserAsync(User user)
    {
        authenticatedUser = user;
    }

    public bool IsAuthenticated()
    {
        if (authenticatedUser != null)
        {
            return true;
        }
        return false;
    }

    public void Logout()
    {
        authenticatedUser = null;
    }


    
}