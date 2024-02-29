using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikasiKeuangan.Finance.FrontEndAPI.ExpensesCategory.DTO
{
    public class ExpensesCategoryDTO
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("categoryName")]
        public string categoryName { get; set; }
    }
}
