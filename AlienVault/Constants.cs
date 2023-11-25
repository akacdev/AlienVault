using System;
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
        /// The base URI to send requests to.
        /// </summary>
        public static readonly Uri BaseUri = new($"https://otx.alienvault.com/api/v{Version}/");
        /// <summary>
        /// The preferred HTTP request version to use.
        /// </summary>
        public static readonly Version HttpVersion = new(2, 0);
        /// <summary>
        /// The maximum delay before considering a request timeout.
        /// </summary>
        public static readonly TimeSpan Timeout = TimeSpan.FromMinutes(1);
        /// <summary>
        /// The value of the <c>User-Agent</c> header to send.
        /// </summary>
        public const string UserAgent = "OTX AlienVault C# Client - actually-akac/AlienVault";
        /// <summary>
        /// The maximum string length when displaying a preview of a response body.
        /// </summary>
        public const int PreviewMaxLength = 500;

        /// <summary>
        /// JSON serializer options used to serialize enums as snake case.
        /// </summary>
        public static readonly JsonSerializerOptions EnumOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter(new SnakeCaseNamingPolicy())
            }
        };
        /// <summary>
        /// JSON serializer options used to serialize indicator enums.
        /// </summary>
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