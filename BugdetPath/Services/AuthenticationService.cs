using BugdetPath.Models;
using System.Text.Json;

namespace BugdetPath.Services;

public class AuthenticationService
{
    private const string AuthFileName = "authenticated_user.json";
    private readonly string authFilePath;
    private User authenticatedUser;

    public AuthenticationService()
    {
        // Set file path for storing the authenticated user
        var dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "data"
        );

        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }

        authFilePath = Path.Combine(dataDirectory, AuthFileName);

        // Load the user from file if it exists
        LoadAuthenticatedUserFromFile();
    }

    public async Task<User> GetAuthenticatedUserAsync()
    {
        return authenticatedUser;
    }

    public async Task SetAuthenticatedUserAsync(User user)
    {
        authenticatedUser = user;

        try
        {
            var json = JsonSerializer.Serialize(user);
            await File.WriteAllTextAsync(authFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving authenticated user: " + ex.Message);
        }
    }

    public bool IsAuthenticated()
    {
        return authenticatedUser != null;
    }

    public void Logout()
    {
        authenticatedUser = null;

        try
        {
            if (File.Exists(authFilePath))
            {
                File.Delete(authFilePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting authentication file: " + ex.Message);
        }
    }

    private void LoadAuthenticatedUserFromFile()
    {
        try
        {
            if (File.Exists(authFilePath))
            {
                var json = File.ReadAllText(authFilePath);
                authenticatedUser = JsonSerializer.Deserialize<User>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading authenticated user: " + ex.Message);
            authenticatedUser = null;
        }
    }
}
