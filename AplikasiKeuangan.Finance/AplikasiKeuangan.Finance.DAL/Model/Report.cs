using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;

namespace AplikasiKeuangan.Finance.DAL.Model
{
    public class Report:ModelBase
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("expenses")]
        public decimal Expenses { get; set; }
        [JsonProperty("expensesCategory")]
        public decimal ExpensesCategory { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }
    }
}
