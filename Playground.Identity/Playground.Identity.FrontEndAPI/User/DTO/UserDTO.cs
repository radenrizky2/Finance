using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Identity.FrontEndAPI.User.DTO
{
    public class UserDTO
    {
        private string _gender;

        [JsonProperty("signUpDate")]
        public DateTime SignUpDate { get; set; }

        [JsonProperty("signUpDateUtc")]
        public DateTime SignUpDateUtc { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("birthday")]
        public DateTime? Birthday { get; set; }

        [JsonProperty("gender")]
        public string Gender
        {
            get { return _gender; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _gender = "";
                }
                else
                {
                    _gender = (value == "M" || value == "F") ? _gender = value :
                        throw new ArgumentException("Invalid gender value. Use 'M' or 'F'");
                }
            }
        }
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("isEmailVerified")]
        public bool IsEmailVerified { get; set; } = false;

        [JsonProperty("mobilePhone")]
        public string MobilePhone { get; set; }


    }
}
