using BugdetPath.Models;

using System.Text.Json;

namespace BugdetPath.Services
{
    public class InflowService : IInflowService
    {
        private readonly string dataDirectory;
        private readonly AuthenticationService authService;

        public InflowService(AuthenticationService authService)
        {
            this.authService = authService;

            dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "data");

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
        }

        // Save a new inflow record
        public async Task SaveInflowAsync(IncomeDetails inflow)
        {
            try
            {
                var user = await authService.GetAuthenticatedUserAsync();
                if (user == null)
                    throw new InvalidOperationException("User is not authenticated.");

                inflow.UserId = user.Id;
                inflow.Date = DateTime.Now;

                var inflows = await LoadUserInflowsAsync(user.Id);

                // Assign a unique ID based on existing inflows
                inflow.Id = inflows.Any() ? inflows.Max(i => i.Id) + 1 : 1;

                inflows.Add(inflow);

                await SaveInflowAsync(user.Id, inflows);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving inflow: {ex.Message}");
                throw;
            }
        }

        // Load all inflows for a specific user
        public async Task<List<IncomeDetails>> LoadUserInflowsAsync(int userId)
        {
            try
            {
                string filePath = GetUserInflowsFilePath(userId);

                if (!File.Exists(filePath))
                    return new List<IncomeDetails>();

                var json = await File.ReadAllTextAsync(filePath);

                return JsonSerializer.Deserialize<List<IncomeDetails>>(json) ?? new List<IncomeDetails>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading inflows: {ex.Message}");
                return new List<IncomeDetails>();
            }
        }

        // Get total amount of inflows for a user
        public async Task<decimal> GetTotalInflowAmountAsync(int userId)
        {
            try
            {
                var inflows = await LoadUserInflowsAsync(userId);
                return inflows.Sum(i => i.Amount); // Assuming Amount is decimal
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating total inflows: {ex.Message}");
                return 0;
            }
        }

        // Save all inflows for a user to file
        private async Task SaveInflowAsync(int userId, List<IncomeDetails> inflows)
        {
            try
            {
                string filePath = GetUserInflowsFilePath(userId);

                var json = JsonSerializer.Serialize(inflows, new JsonSerializerOptions { WriteIndented = true });

                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving inflows: {ex.Message}");
                throw;
            }
        }

        // Helper to get user-specific file path
        private string GetUserInflowsFilePath(int userId)
        {
            return Path.Combine(dataDirectory, $"inflows_{userId}.json");
        }
    }
}
