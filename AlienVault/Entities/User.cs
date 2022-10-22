using System.Text.Json.Serialization;

namespace AlienVault.Entities
{
    /// <summary>
    /// Represents a user on AlienVault.
    /// </summary>
    public class User
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("user_id")]
        public int Id { get; set; }

        [JsonPropertyName("member_since")]
        public string MemberSince { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("subscriber_count")]
        public int SubscriberCount { get; set; }

        [JsonPropertyName("follower_count")]
        public int FollowerCount { get; set; }

        [JsonPropertyName("indicator_count")]
        public int IndicatorCount { get; set; }

        [JsonPropertyName("pulse_count")]
        public int PulseCount { get; set; }

        [JsonPropertyName("award_count")]
        public int AwardCount { get; set; }

        [JsonPropertyName("accepted_edits_count")]
        public int AcceptedEditsCount { get; set; }
    }

    public class ActionResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    /// <summary>
    /// Represents an author on AlienVault.
    /// </summary>
    public class Author
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("is_subscribed")]
        public bool? IsSubscribed { get; set; }

        [JsonPropertyName("is_following")]
        public bool? IsFollowing { get; set; }
    }
}