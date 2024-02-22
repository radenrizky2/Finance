using Newtonsoft.Json;

namespace Playground.Identity.BLL.ProfileManagement
{

    public class RequestDTO
    {

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("edisi")]
        public string Edisi { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("continuationToken")]
        public string ContinuationToken { get; set; }
    }
}
