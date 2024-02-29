using AplikasiKeuangan.Finance.DAL;
using Microsoft.Extensions.Logging;
using Nexus.Base.CosmosDBRepository;

namespace AplikasiKeuangan.Finance.BLL.ExpensesCategoryManagement
{
    public class ExpensesCategroyManager
    {
        private readonly IUnitOfWork _uow;

        public ExpensesCategroyManager(IUnitOfWork uow)
        {
            _uow ??= uow;
        }

        public async Task<DAL.Model.ExpensesCategory> CreateExpensesCategory(DAL.Model.ExpensesCategory category,
            ILogger log, string createdBy = null
            )
        {
            return await _uow.ExpensesCategoryRepository.CreateAsync(category, null, createdBy, null);
        }

        public async Task<DAL.Model.ExpensesCategory> GetExpensesCategoryById(
            string categoryId, ILogger log
            )
        {

            var result = (await _uow.ExpensesCategoryRepository.GetAsync(
                predicate: p => p.Id == categoryId)).Items.FirstOrDefault();

            return result;
        }

        public async Task<PageResult<DAL.Model.ExpensesCategory>> GetAllCategory(
           string continuationToken, ILogger log
           )
        {
            var result = await _uow.ExpensesCategoryRepository.GetAsync();

            return result;
        }

        public async Task<DAL.Model.ExpensesCategory> UpdateExpensesCategory(string categoryId, DAL.Model.ExpensesCategory updatedCategory, ILogger log, string lastUpdatedBy = null, bool isOptimisticConcurrency = false)
        {
            var existingCategory = await _uow.ExpensesCategoryRepository.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                log.LogError($"Category with ID {categoryId} not found.");
                return null;
            }

            existingCategory.CategoryName = updatedCategory.CategoryName;


            var result = await _uow.ExpensesCategoryRepository.UpdateAsync(categoryId, existingCategory, null, lastUpdatedBy, isOptimisticConcurrency);

            return result;
        }

        public async Task DeleteCategory(string categoryId, ILogger log, Dictionary<string, string> partitionKeys = null, EventGridOptions options = null)
        {
            var existingCategory = await _uow.ExpensesCategoryRepository.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                log.LogError($"Category with ID {categoryId} not found.");
                return;
            }

            await _uow.ExpensesCategoryRepository.DeleteAsync(categoryId, partitionKeys, options);
        }
    }
}
