using AplikasiKeuangan.Finance.DAL;
using Microsoft.Extensions.Logging;
using Nexus.Base.CosmosDBRepository;

namespace AplikasiKeuangan.Finance.BLL.BudgetManagement
{
    public class BudgetManager
    {
        private readonly IUnitOfWork _uow;
        public BudgetManager(IUnitOfWork uow)
        {
            _uow ??= uow;
        }

        public async Task<PageResult<DAL.Model.Budget>> GetAllBudget(
           string continuationToken, ILogger log
           )
        {
            var result = await _uow.BudgetRepository.GetAsync();

            return result;
        }

        public async Task<DAL.Model.Budget> CreateBudget(DAL.Model.Budget budget,
            ILogger log, string createdBy = null
            )
        {
            return await _uow.BudgetRepository.CreateAsync(budget, null, createdBy, null);
        }

        public async Task<DAL.Model.Budget> GetBudgetById(string Id, ILogger log)
        {
            var result = (await _uow.BudgetRepository.GetAsync(
                predicate: p => p.Id == Id)).Items.FirstOrDefault();

            return result;
        }

        public async Task<DAL.Model.Budget> GetBudgetByMonthAndYearAsync(int month, int year, ILogger log)
        {
            // Implementasi untuk mendapatkan anggaran berdasarkan bulan dan tahun
            var result = await _uow.BudgetRepository.GetAsync(
                predicate: budget => budget.Month == month && budget.Year == year);

            return result?.Items.FirstOrDefault();
        }

        public async Task<DAL.Model.Budget> UpdateBudget(string budgetId, DAL.Model.Budget updatedBudget, ILogger log, string lastUpdatedBy = null, bool isOptimisticConcurrency = false)
        {
            var existingBudget = await _uow.BudgetRepository.GetByIdAsync(budgetId);
            if (existingBudget == null)
            {
                log.LogError($"Budget with ID {budgetId} not found.");
                return null;
            }

            existingBudget.Amount = updatedBudget.Amount;
            existingBudget.Month = updatedBudget.Month;
            existingBudget.Year = updatedBudget.Year;


            var result = await _uow.BudgetRepository.UpdateAsync(budgetId, existingBudget, null, lastUpdatedBy, isOptimisticConcurrency);

            return result;
        }

        public async Task DeleteBudget(string budgetId, ILogger log, Dictionary<string, string> partitionKeys = null, EventGridOptions options = null)
        {
            var existingUser = await _uow.BudgetRepository.GetByIdAsync(budgetId);
            if (existingUser == null)
            {
                log.LogError($"User with ID {budgetId} not found.");
                return;
            }

            await _uow.BudgetRepository.DeleteAsync(budgetId, partitionKeys, options);
        }

        public async Task<List<DAL.Model.Budget>> GetBudgetsByUserId(string userId, ILogger log)
        {
            var result = await _uow.BudgetRepository.GetAsync(
                predicate: budget => budget.UserId == userId);

            return result.Items.ToList();
        }


    }
}
