using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlienVault.Entities
{
    public class ApiError
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }
    }
}
