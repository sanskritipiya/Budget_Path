using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BugdetPath.Models;

namespace BugdetPath.Services
{
    public class UserInfoService : IUserService
    {
        private readonly string _usersFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "UserDetails.json");

        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {
                var allUsers = await LoadUsersAsync();

                if (allUsers.Any(u => u.UserName == user.UserName))
                    return false; // Username already taken

                user.Id = allUsers.Any() ? allUsers.Max(u => u.Id) + 1 : 1;
                user.Password = HashPassword(user.Password);

                allUsers.Add(user);
                await SaveUsersAsync(allUsers);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                return false;
            }
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                var allUsers = await LoadUsersAsync();
                var hashed = HashPassword(password);

                return allUsers.FirstOrDefault(u => u.UserName == username && u.Password == hashed);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<User>> LoadUsersAsync()
        {
            try
            {
                if (!File.Exists(_usersFilePath))
                    return new List<User>();

                var jsonText = await File.ReadAllTextAsync(_usersFilePath);
                return JsonSerializer.Deserialize<List<User>>(jsonText) ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        public async Task SaveUserAsync(User user)
        {
            try
            {
                var allUsers = await LoadUsersAsync();

                var existingUser = allUsers.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    existingUser.UserName = user.UserName;
                    existingUser.Password = user.Password; // Assume password already hashed if updating
                    // Update other fields if you have more properties
                }
                else
                {
                    user.Id = allUsers.Any() ? allUsers.Max(u => u.Id) + 1 : 1;
                    user.Password = HashPassword(user.Password);
                    allUsers.Add(user);
                }

                await SaveUsersAsync(allUsers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveUserAsync error: {ex.Message}");
                throw;
            }
        }

        // Save all users to file
        private async Task SaveUsersAsync(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_usersFilePath, json);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
