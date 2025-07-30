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

                inflow.Id = inflows.Any() ? inflows.Max(i => i.Id) + 1 : 1;

                inflows.Add(inflow);

                await SaveInflowAsync(user.Id, inflows);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving inflow: {ex.Message}");
                // Removed 'throw;' to prevent unhandled exception
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
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON error loading inflows: {ex.Message}");
                return new List<IncomeDetails>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading inflows: {ex.Message}");
                return new List<IncomeDetails>();
            }
        }

        // Get total inflow amount
        public async Task<decimal> GetTotalInflowAmountAsync(int userId)
        {
            try
            {
                var inflows = await LoadUserInflowsAsync(userId);
                return inflows.Sum(i => i.Amount); // Make sure Amount is decimal
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating total inflow: {ex.Message}");
                return 0;
            }
        }

        // Save inflows to file
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
                Console.WriteLine($"Error writing inflow data: {ex.Message}");
                throw;
            }
        }

        // File path for user inflows
        private string GetUserInflowsFilePath(int userId)
        {
            return Path.Combine(dataDirectory, $"inflows_{userId}.json");
        }
    }
}
