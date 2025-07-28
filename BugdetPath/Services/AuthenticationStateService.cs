using BugdetPath.Models;

namespace BugdetPath.Services;

public class AuthenticationStateService
{
    private UserDetail authenticatedUser;

    public UserDetail GetAuthenticatedUser()
    {
        return authenticatedUser;
    }

    public void SetAuthenticatedUser(UserDetail user)
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