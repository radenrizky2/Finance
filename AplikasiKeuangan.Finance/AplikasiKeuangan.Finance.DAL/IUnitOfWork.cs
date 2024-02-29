using Nexus.Base.CosmosDBRepository;

namespace AplikasiKeuangan.Finance.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IDocumentDBRepository<Model.Budget> BudgetRepository { get; }
        IDocumentDBRepository<Model.ExpensesCategory> ExpensesCategoryRepository { get; }
        IDocumentDBRepository<Model.Expenses> ExpensesRepository { get; }
    }
}
