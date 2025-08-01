using BugdetPath.Components.Pages;
using BugdetPath.Models;

namespace BugdetPath.Services
{
    public interface IDebtService
    {
        Task AddDebtAsync(DebtDetails debt);
    
        Task<List<DebtDetails>> GetDebtsByUserAsync(int userId);
    
        Task<decimal> CalculatePendingDebtAsync(int  userId);
    
        Task<bool> MarkDebtAsClearedAsync(int debtId);
        
        Task<DebtDetails> GetDebtByIdAsync(int debtId, int userId);  

        Task UpdateDebtAsync(DebtDetails debt, int userId);    
    
    
    }
}

