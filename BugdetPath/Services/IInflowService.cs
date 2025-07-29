using BugdetPath.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugdetPath.Services
{
    public interface IInflowService
    {
        Task SaveInflowAsync(IncomeDetails inflow);

        Task<List<IncomeDetails>> LoadUserInflowsAsync(int userId);

        Task<decimal> GetTotalInflowAmountAsync(int userId);
    }
}