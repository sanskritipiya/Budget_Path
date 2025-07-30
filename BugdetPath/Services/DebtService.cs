using BugdetPath.Models;
using System.Text.Json;

namespace BugdetPath.Services
{
    public class DebtService : IDebtService
    {
        private readonly string dataDirectory;
        private readonly AuthenticationService authService;
        private readonly InflowService inflowService;

        public DebtService(AuthenticationService authService, InflowService inflowService)
        {
            this.authService = authService;
            this.inflowService = inflowService;

            dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "data");

            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);
        }

        public async Task AddDebtAsync(DebtDetails debt)
        {
            var user = await authService.GetAuthenticatedUserAsync();
            if (user == null)
                throw new InvalidOperationException("User is not authenticated.");

            debt.UserId = user.Id;
            debt.CreatedAt = DateTime.Now;

            var debts = await LoadUserDebtsAsync(user.Id);
            debt.Id = debts.Any() ? debts.Max(d => d.Id) + 1 : 1;
            debts.Add(debt);

            await SaveDebtsAsync(user.Id, debts);
        }

        public async Task<List<DebtDetails>> GetDebtsByUserAsync(int userId)
        {
            return await LoadUserDebtsAsync(userId);
        }

        public async Task<decimal> CalculatePendingDebtAsync(int userId)
        {
            var debts = await LoadUserDebtsAsync(userId);
            return debts.Where(d => !d.IsCleared).Sum(d => d.Amount);
        }

        public async Task<bool> MarkDebtAsClearedAsync(int debtId)
        {
            var user = await authService.GetAuthenticatedUserAsync();
            if (user == null)
                throw new InvalidOperationException("User not authenticated.");

            var debts = await LoadUserDebtsAsync(user.Id);
            var debt = debts.FirstOrDefault(d => d.Id == debtId && !d.IsCleared);
            if (debt == null)
                return false;

            // Check if inflow can cover this debt
            var totalInflows = await inflowService.GetTotalInflowAmountAsync(user.Id);
            var totalClearedDebt = debts.Where(d => d.IsCleared).Sum(d => d.Amount);
            var availableCash = totalInflows - totalClearedDebt;

            if (availableCash >= debt.Amount)
            {
                debt.IsCleared = true;
                await SaveDebtsAsync(user.Id, debts);
                return true;
            }

            return false; // Not enough inflow to clear debt
        }

        // Private helper to load debts
        private async Task<List<DebtDetails>> LoadUserDebtsAsync(int userId)
        {
            try
            {
                var filePath = GetUserDebtsFilePath(userId);
                if (!File.Exists(filePath))
                    return new List<DebtDetails>();

                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<List<DebtDetails>>(json) ?? new List<DebtDetails>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading debts: {ex.Message}");
                return new List<DebtDetails>();
            }
        }

        // Private helper to save debts
        private async Task SaveDebtsAsync(int userId, List<DebtDetails> debts)
        {
            try
            {
                var filePath = GetUserDebtsFilePath(userId);
                var json = JsonSerializer.Serialize(debts, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving debts: {ex.Message}");
                throw;
            }
        }

        private string GetUserDebtsFilePath(int userId)
        {
            return Path.Combine(dataDirectory, $"debts_{userId}.json");
        }
    }
}
