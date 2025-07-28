using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BugdetPath.Models;

namespace BugdetPath.Services;

    public class UserInfoService : IUserService
    {
        private readonly string _usersFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "UserDetails.json");

        //save a new user to the system 
        public async Task SaveUserAsync(UserDetail user)
        {
            try
            {
                var allUsers = await LoadUsersAsync();

                // Auto-assign ID if needed
                user.Id = allUsers.Count > 0 ? allUsers.Max(u => u.Id) + 1 : 1;

                // Secure the password
                user.Password = HashPassword(user.Password);

                allUsers.Add(user);

                await SaveUserAsync(allUsers);
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the user: {ex.Message}");
                throw;
            }
        }

        public async Task<List<UserDetail>> LoadUsersAsync()
        {
            try
            {
                var jsonText = await File.ReadAllTextAsync(_usersFilePath);
                
                return JsonSerializer.Deserialize<List<UserDetail>>(jsonText) ?? new List<UserDetail>();
            }
            catch (JsonException jex)
            {
                Console.WriteLine($"Error parsing JSON data: {jex.Message}");
                return new List<UserDetail>();
            }
            catch (IOException ioex)
            {
                Console.WriteLine($"File access error: {ioex.Message}");
                return new List<UserDetail>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new List<UserDetail>();
            }
        }

        private async Task SaveUserAsync(List<UserDetail> users)
        {
            try
            {
               var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_usersFilePath, json);
            }
            catch (Exception e)
            {
                        Console.WriteLine(e);
                        throw;
            }
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            
            var bytes = Encoding.UTF8.GetBytes(password);
            
            var hashed = sha.ComputeHash(bytes);
            
            return Convert.ToBase64String(hashed);
        }
    }
