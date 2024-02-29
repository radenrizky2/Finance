using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikasiKeuangan.Finance.FrontEndAPI.Expenses.DTO
{
    public class ExpensesDTO
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
