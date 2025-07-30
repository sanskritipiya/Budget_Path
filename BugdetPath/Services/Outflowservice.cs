using BugdetPath.Models;
using System.Text.Json;

namespace BugdetPath.Services
{
    public class OutflowService : IOutflowService
    {
        private readonly string dataDirectory;
        private readonly AuthenticationService authService;

        public OutflowService(AuthenticationService authService)
        {
            this.authService = authService;

            dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "data");

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
        }

        // ✅ Method name fixed to match interface
        public async Task SaveOutflowsAsync(ExpenseDetails outflow)
        {
            try
            {
                var user = await authService.GetAuthenticatedUserAsync();
                if (user == null)
                    throw new InvalidOperationException("User is not authenticated.");

                outflow.UserId = user.Id;
                outflow.Date = DateTime.Now;

                var outflows = await LoadUserOutflowsAsync(user.Id);

                outflow.Id = outflows.Any() ? outflows.Max(i => i.Id) + 1 : 1;

                outflows.Add(outflow);

                await SaveOutflowsToFileAsync(user.Id, outflows); // ✅ Renamed for clarity
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured while saving outflow: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ExpenseDetails>> LoadUserOutflowsAsync(int userId)
        {
            try
            {
                string filePath = GetUserOutflowsFilePath(userId);

                if (!File.Exists(filePath))
                {
                   return new List<ExpenseDetails>();
                }
                
                var json = await File.ReadAllTextAsync(filePath);

                return JsonSerializer.Deserialize<List<ExpenseDetails>>(json) ?? new List<ExpenseDetails>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading outflows: {ex.Message}");
                return new List<ExpenseDetails>();
            }
        }

        public async Task<decimal> GetTotalOutflowAmountAsync(int userId)
        {
            try
            {
                var outflows = await LoadUserOutflowsAsync(userId);
                return outflows.Sum(i => i.Amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured while calculating total outflows: {ex.Message}");
                return 0;
            }
        }

        // ✅ Renamed to avoid confusion with interface method
        private async Task SaveOutflowsToFileAsync(int userId, List<ExpenseDetails> outflows)
        {
            try
            {
                string filePath = GetUserOutflowsFilePath(userId);

                var json = JsonSerializer.Serialize(outflows, new JsonSerializerOptions { WriteIndented = true });

                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving outflows: {ex.Message}");
                throw;
            }
        }

        private string GetUserOutflowsFilePath(int userId)
        {
            return Path.Combine(dataDirectory, $"outflows_{userId}.json");
        }
    }
}
