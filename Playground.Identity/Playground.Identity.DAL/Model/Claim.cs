using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Identity.DAL.Model
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
