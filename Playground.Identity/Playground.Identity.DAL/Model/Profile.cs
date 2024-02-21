using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;

namespace Playground.Identity.DAL.Model
{
    public class Profile : ModelBase
    {

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("edisi")]
        public string Edisi { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }
}
