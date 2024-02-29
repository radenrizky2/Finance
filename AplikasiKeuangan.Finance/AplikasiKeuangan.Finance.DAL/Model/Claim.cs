using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;

namespace AplikasiKeuangan.Finance.DAL.Model
{
    public class Claim : ModelBase
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("claimType")]
        public string ClaimType { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
