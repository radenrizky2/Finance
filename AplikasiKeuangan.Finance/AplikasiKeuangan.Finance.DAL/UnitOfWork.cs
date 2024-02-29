using Microsoft.Azure.Cosmos;
using Nexus.Base.CosmosDBRepository;

namespace AplikasiKeuangan.Finance.DAL
{
    public class UnitOfWork : IUnitOfWork
    {

        private static readonly string C_EventGridEndPoint = Environment.GetEnvironmentVariable("EventGridEndPoint");
        private static readonly string C_EventGridKey = Environment.GetEnvironmentVariable("EventGridKey");
        private static readonly string _DB1 = "Finance";
        private readonly CosmosClient _client;

        private readonly Lazy<IDocumentDBRepository<Model.Budget>> budgetRepository;
        private readonly Lazy<IDocumentDBRepository<Model.ExpensesCategory>> expensesCategoryRepository;
        private readonly Lazy<IDocumentDBRepository<Model.Expenses>> expensesRepository;


        public UnitOfWork(CosmosClient client)
        {
            _client = client;

            budgetRepository ??= new Lazy<IDocumentDBRepository<Model.Budget>>(new DocumentDBRepository<DAL.Model.Budget>
              (_DB1, _client, eventGridEndPoint: C_EventGridEndPoint, eventGridKey: C_EventGridKey));

            expensesCategoryRepository ??= new Lazy<IDocumentDBRepository<Model.ExpensesCategory>>(new DocumentDBRepository<DAL.Model.ExpensesCategory>
             (_DB1, _client, eventGridEndPoint: C_EventGridEndPoint, eventGridKey: C_EventGridKey));

            expensesRepository ??= new Lazy<IDocumentDBRepository<Model.Expenses>>(new DocumentDBRepository<DAL.Model.Expenses>
             (_DB1, _client, eventGridEndPoint: C_EventGridEndPoint, eventGridKey: C_EventGridKey));

        }
        public IDocumentDBRepository<Model.Budget> BudgetRepository => budgetRepository.Value;
        public IDocumentDBRepository<Model.ExpensesCategory> ExpensesCategoryRepository => expensesCategoryRepository.Value;
        public IDocumentDBRepository<Model.Expenses> ExpensesRepository => expensesRepository.Value;


        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
