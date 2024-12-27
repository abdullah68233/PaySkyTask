using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Entities
{
    public class UserTokenDTO
    {


        public UserTokenDTO(string token, DateTime expiryDate, DateTime creationDate)
        {
            Token = token;
            CreationDate = creationDate.ToUniversalTime().ToString("o");
            ExpiryDate = expiryDate.ToUniversalTime().ToString("o");
        }

        public string Token { get; set; }
        [JsonPropertyName("expiresin")]
        public string ExpiryDate { get; set; }
        [JsonPropertyName("createdAt")]
        public string CreationDate { get; set; }

    }
}
