using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;

namespace AplikasiKeuangan.Finance.DAL.Model
{
    public class ExpensesCategory : ModelBase
    {

        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }
    }
}
