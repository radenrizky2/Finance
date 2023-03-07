using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Identity.FrontEndAPI.JWT
{
    public class JWTDTO
    {
        [JsonProperty("jwt")]
        public string JWT { get; set; }

        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }
    }
}
