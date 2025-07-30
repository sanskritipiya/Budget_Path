using System;
using BugdetPath.Models;
using BugdetPath.Services;

namespace BugdetPath.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IInflowService inflowService;
        private readonly IOutflowService outflowService;
        private readonly IDebtService debtService;
        private readonly AuthenticationService authService;

        private readonly string dataPath;

        public TransactionService(
            IInflowService incomeService,
            IOutflowService expensesService,
            IDebtService debtService,
            AuthenticationService authService)
        {
            this.inflowService = incomeService;
            this.outflowService = expensesService;
            this.debtService = debtService;
            this.authService = authService;

            // macOS-compatible file location for user data
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dataPath = Path.Combine(appData, "Budgetify", "transactions");

            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
        }

        // Combines all income, expense, and debt records into a single list
        public Task<List<TransactionDetails>> FetchAllTransactionsAsync()
        {
            throw new NotImplementedException();
        }

        // Filters by transaction type
        public async Task<List<TransactionDetails>> RetrieveTransactionsByTypeAsync(string type)
        {
            var all = await FetchAllTransactionsAsync();
            return all.Where(t => t.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Gets most recent N transactions
        public Task<List<TransactionDetails>> FetchTopTransactionsAsync(int topCount)
        {
            throw new NotImplementedException();
        }

        // Gets total number of transactions
        public async Task<int> GetTotalTransactionsCountAsync()
        {
            var all = await FetchAllTransactionsAsync();
            return all.Count;
        }

        // Updates a transaction (based on type)
        public async Task UpdateTransactionAsync(TransactionDetails transaction)
        {
            var user = await authService.GetAuthenticatedUserAsync();
            if (user == null)
                throw new Exception("Not authenticated.");

            if (transaction.Type == "Income")
            {
                var all = await inflowService.LoadUserInflowsAsync(user.Id);
                var match = all.FirstOrDefault(x => x.Id == transaction.Id);
                if (match != null)
                {
                    match.Title = transaction.Title;
                    match.Amount = transaction.Amount;
                    match.Notes = transaction.Notes;
                    match.Date = transaction.Date;
                    match.Tag = transaction.Tag;

                    await inflowService.SaveInflowAsync(match);
                }
            }

            if (transaction.Type == "Expense")
            {
                var all = await outflowService.LoadUserOutflowsAsync(user.Id);
                var match = all.FirstOrDefault(x => x.Id == transaction.Id);
                if (match != null)
                {
                    match.Title = transaction.Title;
                    match.Amount = transaction.Amount;
                    match.Notes = transaction.Notes;
                    match.Date = transaction.Date;
                    match.Tag = transaction.Tag;

                    await outflowService.SaveOutflowsAsync(match);
                }
            }

            if (transaction.Type == "Debt")
            {
                var all = await debtService.GetDebtsByUserAsync(user.Id);
                var match = all.FirstOrDefault(x => x.Id == transaction.Id);
                if (match != null)
                {
                    match.Title = transaction.Title;
                    match.Amount = transaction.Amount;
                    match.Notes = transaction.Notes;
                    match.Date = transaction.Date;

                    await debtService.UpdateDebtAsync(match, user.Id);
                }
            }
        }

        // Mark debt as cleared
        public async Task ClearDebtAsync(int debtId, int userId)
        {
            var debt = await debtService.GetDebtByIdAsync(debtId, userId);
            if (debt == null)
                throw new Exception("Debt not found.");

            debt.IsCleared = true;
            await debtService.UpdateDebtAsync(debt, userId);
        }

        // Update multiple debts (if cleared)
        public async Task UpdateDebtsAsync(List<DebtDetails> debtList)
        {
            var user = await authService.GetAuthenticatedUserAsync();
            if (user == null)
                throw new Exception("Not authenticated.");

            foreach (var d in debtList)
            {
                if (d.IsCleared)
                    await debtService.UpdateDebtAsync(d, user.Id);
            }
        }

        // Save or update a single debt
        public async Task UpdateDebtAsync(DebtDetails debt)
        {
            var user = await authService.GetAuthenticatedUserAsync();
            if (user == null)
                throw new Exception("Not authenticated.");

            await debtService.UpdateDebtAsync(debt, user.Id);
        }

        // Utility to convert each record type to a common transaction record
        private List<TransactionDetails> ConvertToTransactionRecords<T>(List<T> source, string type, int userId)
        {
            var result = new List<TransactionDetails>();

            foreach (var record in source)
            {
                if (record is IncomeDetails i)
                {
                    result.Add(new TransactionDetails
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Amount = i.Amount,
                        Notes = i.Notes,
                        Tag = i.Tag,
                        Date = i.Date,
                        Type = type,
                        UserId = userId
                    });
                }
                else if (record is ExpenseDetails e)
                {
                    result.Add(new TransactionDetails
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Amount = e.Amount,
                        Notes = e.Notes,
                        Tag = e.Tag,
                        Date = e.Date,
                        Type = type,
                        UserId = userId
                    });
                }
                else if (record is DebtDetails d)
                {
                    result.Add(new TransactionDetails
                    {
                        Id = d.Id,
                        Title = d.Title,
                        Amount = d.Amount,
                        Notes = d.Notes,
                        Date = d.Date,
                        Type = type,
                        UserId = userId
                    });
                }
            }

            return result;
        }
    }
}
