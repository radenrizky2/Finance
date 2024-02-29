using Newtonsoft.Json;
using System.Collections.Generic;

namespace AplikasiKeuangan.Finance.FrontEndAPI.JWT
{
    public class UserClaimsDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("nameIdentifier")]
        public string NameIdentifier { get; set; }
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
        // Add any other properties you need here
    }
}
