using System.Text.Json.Serialization;

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