using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlienVault
{
    internal static class Constants
    {
        /// <summary>
        /// The version of the API to use.
        /// </summary>
        public const int Version = 1;
        /// <summary>
        /// The value of the <c>User-Agent</c> header to send.
        /// </summary>
        public const string UserAgent = "OTX AlienVault C# Client - actually-akac/AlienVault";

        public static readonly JsonSerializerOptions EnumOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter(new SnakeCaseNamingPolicy())
            }
        };

        public static readonly JsonSerializerOptions IndicatorOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter(new IndicatorNamingPolicy())
            }
        };

        public const int UserSearchSize = 20;
        public const int PulseSearchSize = 10;
        public const int IndicatorGetSize = 1000;
        public const int RelatedPulsesSize = 5;
        public const int SubscribedPulsesSize = 10;
        public const int SubscribedPulseIdsSize = 1000;
        public const int ActivityPulsesSize = 10;
        public const int EventsSize = 100;
        public const int PulseFeedSize = 10;
        public const int SubmittedFilesSize = 100;
        public const int SubmittedURLsSize = 100;
        public const int MalwareSize = 50;
        public const int AssociatedURLsSize = 100;
    }
}