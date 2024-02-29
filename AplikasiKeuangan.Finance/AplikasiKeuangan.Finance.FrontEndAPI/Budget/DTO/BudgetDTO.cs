using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikasiKeuangan.Finance.FrontEndAPI.Budget.DTO
{
    public class BudgetDTO
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

    }
}
