using BugdetPath.Models;

namespace BugdetPath.Services;

public class AuthenticationStateService
{
    private User authenticatedUser;

    public User GetAuthenticatedUser()
    {
        return authenticatedUser;
    }

    public void SetAuthenticatedUser(User user)
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