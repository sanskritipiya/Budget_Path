using BugdetPath.Models;

namespace BugdetPath.Services;

public interface ITransactionService
{
    Task<List<TransactionDetails>>FetchAllTransactionsAsync();
    Task<List<TransactionDetails>>FetchTopTransactionsAsync(int topCount);
    
    Task<List<TransactionDetails>> RetrieveTransactionsByTypeAsync(string transactionType);  // Filtering method
    Task UpdateTransactionAsync(TransactionDetails transaction);  // Method to update a transaction
    Task UpdateDebtsAsync(List<DebtDetails> debts);
    Task<int> GetTotalTransactionsCountAsync();
    Task UpdateDebtAsync(DebtDetails debt);
    Task ClearDebtAsync(int debtId, int userId);  // Clear debt method for a specific debt
}