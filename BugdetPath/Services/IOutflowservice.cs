using BugdetPath.Models;

namespace BugdetPath.Services

{
    public interface IOutflowService
    {
        Task SaveOutflowsAsync(ExpenseDetails outflow);

        Task<List<ExpenseDetails>> LoadUserOutflowsAsync(int userId);

        Task<decimal> GetTotalOutflowAmountAsync(int userId);
    }
}

