using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Identity.FrontEndAPI.Profile
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
