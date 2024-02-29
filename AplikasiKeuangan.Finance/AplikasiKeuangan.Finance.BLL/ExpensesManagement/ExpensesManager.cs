using AplikasiKeuangan.Finance.BLL.ExpensesCategoryManagement;
using AplikasiKeuangan.Finance.DAL;
using AplikasiKeuangan.Finance.DAL.Model;
using Microsoft.Extensions.Logging;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikasiKeuangan.Finance.BLL.ExpensesManagement
{
    public class ExpensesManager
    {
        private readonly IUnitOfWork _uow;
        private readonly ExpensesCategroyManager _expensesCategoryManager;


        public ExpensesManager(IUnitOfWork uow)
        {
            _uow ??= uow;
            _expensesCategoryManager ??= new ExpensesCategroyManager(_uow);
        }


        public async Task<bool> CategoryExists(string categoryId, ILogger log)
        {
            var category = await _expensesCategoryManager.GetExpensesCategoryById(categoryId, log);
            return category != null;
        }

        public async Task<DAL.Model.Expenses> CreateExpenses(DAL.Model.Expenses expenses,
            ILogger log, string createdBy = null
            )
        {
            return await _uow.ExpensesRepository.CreateAsync(expenses, null, createdBy, null);
        }

        public async Task<DAL.Model.Expenses> GetExpensesById(
            string expensesId, ILogger log
            )
        {

            var result = (await _uow.ExpensesRepository.GetAsync(
                predicate: p => p.Id == expensesId)).Items.FirstOrDefault();

            return result;
        }

        public async Task<PageResult<DAL.Model.Expenses>> GetAllExpenses(
           string continuationToken, ILogger log
           )
        {
            var result = await _uow.ExpensesRepository.GetAsync();

            return result;
        }

        public async Task<DAL.Model.Expenses> UpdateExpenses(string expensesId, DAL.Model.Expenses updatedExpenses, ILogger log, string lastUpdatedBy = null, bool isOptimisticConcurrency = false)
        {
            var existingExpenses = await _uow.ExpensesRepository.GetByIdAsync(expensesId);
            if (existingExpenses == null)
            {
                log.LogError($"User with ID {expensesId} not found.");
                return null;
            }

            existingExpenses.ExpensesName = updatedExpenses.ExpensesName;
            existingExpenses.Amount = updatedExpenses.Amount;
            existingExpenses.Notes = updatedExpenses.Notes;
            existingExpenses.CategoryId = updatedExpenses.CategoryId;


            var result = await _uow.ExpensesRepository.UpdateAsync(expensesId, existingExpenses, null, lastUpdatedBy, isOptimisticConcurrency);

            return result;
        }

        public async Task DeleteExpenses(string expensesId, ILogger log, Dictionary<string, string> partitionKeys = null, EventGridOptions options = null)
        {
            var existingExpenses = await _uow.ExpensesRepository.GetByIdAsync(expensesId);
            if (existingExpenses == null)
            {
                log.LogError($"Expenses with ID {expensesId} not found.");
                return;
            }

            await _uow.ExpensesRepository.DeleteAsync(expensesId, partitionKeys, options);
        }

        internal object Where(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}
