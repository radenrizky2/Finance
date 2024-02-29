using AplikasiKeuangan.Finance.BLL.BudgetManagement;
using AplikasiKeuangan.Finance.BLL.ExpensesCategoryManagement;
using AplikasiKeuangan.Finance.BLL.ExpensesManagement;
using AplikasiKeuangan.Finance.DAL;
using Newtonsoft.Json;

namespace AplikasiKeuangan.Expenses.BLL.Report
{
    public class ReportManager
    {
        private readonly IUnitOfWork _uow;
        private readonly ExpensesCategroyManager _expensesCategoryManager;
        private readonly ExpensesManager _expensesManager;
        private readonly BudgetManager _budgetManager;

        public ReportManager(IUnitOfWork uow)
        {
            _uow = uow;
            _expensesCategoryManager = new ExpensesCategroyManager(_uow);
            _expensesManager = new ExpensesManager(_uow);
            _budgetManager = new BudgetManager(_uow);
        }

        // Metode untuk menghitung total pengeluaran dalam rentang waktu tertentu
        private async Task<decimal> CalculateTotalExpensesAsync(IEnumerable<Finance.DAL.Model.Expenses> expenses)
        {
            return expenses.Sum(expense => expense.Amount);
        }

        // Metode untuk menghitung sisa anggaran berdasarkan total pengeluaran dan anggaran tersedia
        private async Task<decimal> CalculateRemainingBudgetAsync(decimal totalExpenses, decimal budget)
        {
            return budget - totalExpenses;
        }

        // Metode untuk menghitung report keuangan harian
        public async Task<string> GenerateDailyReportAsync(DateTime date)
        {
            var dailyExpenses = await _uow.ExpensesRepository.GetAsync(
                selector: s => s,
                predicate: p => p.DateExpenses.Date == date.Date,
                usePaging: false
            );
            var totalExpenses = await CalculateTotalExpensesAsync(dailyExpenses.Items);

            var monthlyBudgetResult = await _uow.BudgetRepository.GetAsync(
                selector: b => b,
                predicate: budget => budget.Month == date.Month && budget.Year == date.Year,
                usePaging: false
            );
            var dailyBudgetAmount = monthlyBudgetResult?.Items.FirstOrDefault()?.Amount ?? 0;

            var remainingBudget = await CalculateRemainingBudgetAsync(totalExpenses, dailyBudgetAmount);

            var report = new
            {
                Date = date,
                TotalExpenses = totalExpenses,
                DailyBudget = dailyBudgetAmount,
                RemainingBudget = remainingBudget
            };

            return JsonConvert.SerializeObject(report);
        }



        public async Task<string> GenerateMonthlyReportAsync(int month, int year)
        {
            var monthlyExpenses = await _uow.ExpensesRepository.GetAsync(
                selector: s => s,
                predicate: p => p.DateExpenses.Month == month && p.DateExpenses.Year == year,
                usePaging: false
            );
            var totalExpenses = await CalculateTotalExpensesAsync(monthlyExpenses.Items);

            var monthlyBudgetResult = await _uow.BudgetRepository.GetAsync(
                selector: b => b,
                predicate: budget => budget.Month == month && budget.Year == year,
                usePaging: false
            );
            var monthlyBudget = monthlyBudgetResult?.Items.Sum(b => b.Amount) ?? 0;

            var remainingBudget = await CalculateRemainingBudgetAsync(totalExpenses, monthlyBudget);

            var report = new
            {
                Month = month,
                Year = year,
                TotalExpenses = totalExpenses,
                MonthlyBudget = monthlyBudget,
                RemainingBudget = remainingBudget
            };

            return JsonConvert.SerializeObject(report);
        }

        public async Task<string> GenerateYearlyReportAsync(int year)
        {
            var yearlyExpenses = await _uow.ExpensesRepository.GetAsync(
                selector: s => s,
                predicate: p => p.DateExpenses.Year == year,
                usePaging: false
            );
            var totalExpenses = await CalculateTotalExpensesAsync(yearlyExpenses.Items);

            var yearlyBudgetResult = await _uow.BudgetRepository.GetAsync(
                selector: b => b,
                predicate: budget => budget.Year == year,
                usePaging: false
            );
            var yearlyBudget = yearlyBudgetResult?.Items.Sum(budget => budget.Amount) ?? 0;

            var remainingBudget = await CalculateRemainingBudgetAsync(totalExpenses, yearlyBudget);

            var report = new
            {
                Year = year,
                TotalExpenses = totalExpenses,
                YearlyBudget = yearlyBudget,
                RemainingBudget = remainingBudget
            };

            return JsonConvert.SerializeObject(report);
        }
    }
}