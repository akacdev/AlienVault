using AlienVault.Entities;
using AlienVault.Modules;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace AlienVault
{
    /// <summary>
    /// The primary class for interacting with the AlienVault OTX DirectConnect API.
    /// </summary>
    public class AlienVaultClient
    {
        /// <summary>
        /// The base URI to use when communicating.
        /// </summary>
        public static readonly Uri BaseUri = new($"https://otx.alienvault.com/api/v{Constants.Version}/");

        private static readonly HttpClientHandler HttpHandler = new()
        {
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = false
        };

        private readonly HttpClient Client = new(HttpHandler)
        {
            BaseAddress = BaseUri,
            DefaultRequestVersion = new(2, 0),
            Timeout = TimeSpan.FromMinutes(5)
        };

        private readonly AlienVaultClientConfig Config;

        /// <summary>
        /// Create a new instance of the client.
        /// </summary>
        /// <param name="config">Configuration properties for this client.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AlienVaultClient(AlienVaultClientConfig config)
        {
            if (config is null) throw new ArgumentNullException(nameof(config), "Provided AlienVault Client config is null.");
            if (string.IsNullOrEmpty(config.Key)) throw new ArgumentNullException(nameof(config.Key), "API key is null or empty.");

            Config = config;

            Client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.UserAgent);
            Client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            Client.DefaultRequestHeaders.Accept.ParseAdd("text/html");
            Client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
            Client.DefaultRequestHeaders.Add("X-OTX-API-Key", config.Key);

            Users = new(Client, Config);
            Search = new(Client, Config);
            Pulses = new(Client, Config);
            Indicators = new(Client, Config);
            Data = new(Client, Config);
            Analysis = new(Client, Config);
        }

        /// <summary>
        /// Interact with users on AlienVault.
        /// </summary>
        public readonly UserModule Users;
        /// <summary>
        /// Interact with search on AlienVault.
        /// </summary>
        public readonly SearchModule Search;
        /// <summary>
        /// Interact with pulses on AlienVault.
        /// </summary>
        public readonly PulseModule Pulses;
        /// <summary>
        /// Interact with indicators on AlienVault.
        /// </summary>
        public readonly IndicatorModule Indicators;
        /// <summary>
        /// Interact with data endpoints on AlienVault.
        /// </summary>
        public readonly DataModule Data;
        /// <summary>
        /// Interact with analysis endpoints on AienVault.
        /// </summary>
        public readonly AnalysisModule Analysis;
    }
}