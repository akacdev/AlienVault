using System.Text.Json.Serialization;

namespace AlienVault.Entities
{
    /// <summary>
    /// Provides sort options for users.
    /// </summary>
    public enum UserSort
    {
        Username,
        PulseCount
    }

    /// <summary>
    /// Provides sort options for pulses.
    /// </summary>
    public enum PulseSort
    {
        Modified,
        Created,
        SubscriberCount
    }

    public class UserSearchPage
    {
        [JsonPropertyName("results")]
        public User[] Results { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("previous")]
        public string Previous { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }
    }

    public class PulseSearchPage
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("results")]
        public Pulse[] Results { get; set; }

        [JsonPropertyName("previous")]
        public string Previous { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }
    }
}