using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;

namespace AplikasiKeuangan.Finance.DAL.Model
{
    public class Expenses : ModelBase
    {

        [JsonProperty("expensesName")]
        public string ExpensesName { get; set; }
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }
        [JsonProperty("dateExpenses")]
        public DateTime DateExpenses { get; set; }
        [JsonProperty("notes")]
        public string? Notes { get; set; }
    }
}
