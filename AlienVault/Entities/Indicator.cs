using System.Text.Json.Serialization;

namespace AlienVault.Entities
{
    public class IndicatorTypesContainer
    {
        [JsonPropertyName("detail")]
        public IndicatorTypeInfo[] Detail { get; set; }
    }

    public class IndicatorTypeInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }

    public class IndicatorValidation : Indicator
    {
        [JsonPropertyName("access_type")]
        public string AccessType { get; set; }

        [JsonPropertyName("access_reason")]
        public string AccessReason { get; set; }

        [JsonPropertyName("access_groups")]
        public Group[] AccessGroups { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("validation")]
        public Validation[] Result { get; set; }
    }

    public class Validation
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}