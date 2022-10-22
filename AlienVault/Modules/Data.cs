using AlienVault.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlienVault.Modules
{
    public class DataModule
    {
        private readonly HttpClient Client;
        private readonly AlienVaultClientConfig Config;

        public DataModule(HttpClient client, AlienVaultClientConfig config)
        {
            Client = client;
            Config = config;
        }

        public static int ParseIP(string ip)
        {
            bool success = IPAddress.TryParse(ip, out IPAddress addr);
            if (!success) throw new ArgumentException($"Provided IP address ({ip}) is invalid.", nameof(ip));

            int version;

            #pragma warning disable IDE0066
            switch (addr.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:
                    version = 4;
                    break;
                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                    version = 6;
                    break;
                default:
                    throw new ArgumentException($"Provided IP address ({ip}) has to be either IPv4 or IPv6.", nameof(ip));
            }
            #pragma warning restore IDE0066

            return version;
        }

        /// <summary>
        /// Get general information about an IP address.
        /// </summary>
        /// <param name="ip">The target IP address.</param>
        /// <returns>An instance of <see cref="GeneralIPInfo"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<GeneralIPInfo> GetGeneralIPInfo(string ip)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/IPv{ParseIP(ip)}/{ip}/general");
            return await res.Deseralize<GeneralIPInfo>();
        }

        /// <summary>
        /// Get general information about a domain.
        /// </summary>
        /// <param name="domain">The target domain.</param>
        /// <returns>An instance of <see cref="GeneralDomainInfo"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<GeneralDomainInfo> GetGeneralDomainInfo(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException(nameof(domain), "Domain is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/domain/{domain}/general");
            return await res.Deseralize<GeneralDomainInfo>();
        }

        /// <summary>
        /// Get geolocation and ISP information about an IP address.
        /// </summary>
        /// <param name="ip">The target IP address.</param>
        /// <returns>An instance of <see cref="Geolocation"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Geolocation> GetIPGeo(string ip)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/IPv{ParseIP(ip)}/{ip}/geo");
            return await res.Deseralize<Geolocation>();
        }

        /// <summary>
        /// Get geolocation and ISP information about a domain's underlying IP address.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Geolocation> GetDomainGeo(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException(nameof(domain), "Domain is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/domain/{domain}/geo");
            return await res.Deseralize<Geolocation>();
        }

        /// <summary>
        /// Get malware samples associated with an IP address.
        /// </summary>
        /// <param name="ip">The target IP address.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="Malware"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Malware[]> GetIPMalware(string ip, int limit = Constants.MalwareSize)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            int version = ParseIP(ip);

            List<Malware> output = new(limit % Constants.MalwareSize == 0 ? limit : limit + Constants.MalwareSize);

            int pageLimit = limit < Constants.MalwareSize ? limit : Constants.MalwareSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"indicators/IPv{version}/{ip}/malware?limit={pageLimit}&page={page}");

                MalwarePage chunk = await res.Deseralize<MalwarePage>();
                output.AddRange(chunk.Results);

                if (output.Count >= limit || chunk.Count == 0 || chunk.Size == 0 || chunk.Results.Length == 0) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get malware samples associated with a domain.
        /// </summary>
        /// <param name="domain">The target domain.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="Malware"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Malware[]> GetDomainMalware(string domain, int limit = Constants.MalwareSize)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException(nameof(domain), "Domain is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<Malware> output = new(limit % Constants.MalwareSize == 0 ? limit : limit + Constants.MalwareSize);

            int pageLimit = limit < Constants.MalwareSize ? limit : Constants.MalwareSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"indicators/domain/{domain}/malware?limit={pageLimit}&page={page}");

                MalwarePage chunk = await res.Deseralize<MalwarePage>();
                output.AddRange(chunk.Results);

                if (output.Count >= limit || chunk.Count == 0 || chunk.Size == 0 || chunk.Results.Length == 0) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get URLs associated with an IP address.
        /// </summary>
        /// <param name="ip">The target IP address.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="AssociatedURL"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<AssociatedURL[]> GetIPAssociatedURLs(string ip, int limit = Constants.AssociatedURLsSize)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            int version = ParseIP(ip);

            List<AssociatedURL> output = new(limit % Constants.AssociatedURLsSize == 0 ? limit : limit + Constants.AssociatedURLsSize);

            int pageLimit = limit < Constants.AssociatedURLsSize ? limit : Constants.AssociatedURLsSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"indicators/IPv{version}/{ip}/url_list?limit={pageLimit}&page={page}");

                AssociatedURLsPage chunk = await res.Deseralize<AssociatedURLsPage>();
                output.AddRange(chunk.Results);

                if (output.Count >= limit || !chunk.HasNext || chunk.FullSize == 0 || chunk.ActualSize == 0 || chunk.Results.Length == 0) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get URLs associated with a domain.
        /// </summary>
        /// <param name="domain">The target domain.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="AssociatedURL"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<AssociatedURL[]> GetDomainAssociatedURLs(string domain, int limit = Constants.AssociatedURLsSize)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException(nameof(domain), "Domain is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<AssociatedURL> output = new(limit % Constants.AssociatedURLsSize == 0 ? limit : limit + Constants.AssociatedURLsSize);

            int pageLimit = limit < Constants.AssociatedURLsSize ? limit : Constants.AssociatedURLsSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"indicators/domain/{domain}/url_list?limit={pageLimit}&page={page}");

                AssociatedURLsPage chunk = await res.Deseralize<AssociatedURLsPage>();
                output.AddRange(chunk.Results);

                if (output.Count >= limit || !chunk.HasNext || chunk.FullSize == 0 || chunk.ActualSize == 0 || chunk.Results.Length == 0) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get passive DNS resolutions associated with the provided IP address.
        /// </summary>
        /// <param name="ip">The target IP address.</param>
        /// <returns>An array of <see cref="PassiveDNS"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<PassiveDNS[]> GetPassiveIPDNS(string ip)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/IPv{ParseIP(ip)}/{ip}/passive_dns");
            return (await res.Deseralize<PassiveDNSContainer>()).Results;
        }

        /// <summary>
        /// Get passive DNS resolutions associated with the provided domain.
        /// </summary>
        /// <param name="domain">The target domain.</param>
        /// <returns>An array of <see cref="PassiveDNS"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<PassiveDNS[]> GetPassiveDomainDNS(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException(nameof(domain), "Domain is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/domain/{domain}/passive_dns");
            return (await res.Deseralize<PassiveDNSContainer>()).Results;
        }

        /// <summary>
        /// Get WHOIS information about a domain.
        /// </summary>
        /// <param name="domain">The target domain.</param>
        /// <returns>An array of <see cref="WhoisEntry"/>.</returns>
        public async Task<WhoisEntry[]> GetWhois(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException(nameof(domain), "Domain is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/domain/{domain}/whois");
            return (await res.Deseralize<WhoisEntriesContainer>()).Results;
        }

        /// <summary>
        /// Get HTTP scans of an IP address.
        /// </summary>
        /// <param name="ip">The target IP address.</param>
        /// <returns>An array of <see cref="HTTPScan"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AlienVaultException"></exception>
        public async Task<HTTPScan[]> GetHTTPScans(string ip)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/IPv{ParseIP(ip)}/{ip}/http_scans");
            HTTPScanContainer container = await res.Deseralize<HTTPScanContainer>();
            if (container.Error is not null) throw new AlienVaultException($"Received the following error message when requesting HTTP scans for '{ip}': {container.Error}");

            return container.Data;
        }

        /// <summary>
        /// Get a file indicator by its hash.
        /// </summary>
        /// <param name="hash">The target file's hash.</param>
        /// <returns>An instance of <see cref="FileIndicator"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<FileIndicator> GetFileIndicator(string hash)
        {
            if (string.IsNullOrEmpty(hash)) throw new ArgumentNullException(nameof(hash), "File hash is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/file/{hash}/general", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;

            return await res.Deseralize<FileIndicator>();
        }

        /// <summary>
        /// Get a URL indicator.
        /// </summary>
        /// <param name="url">The target URL.</param>
        /// <returns>An instance of <see cref="URLIndicator"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<URLIndicator> GetURLIndicator(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url), "URL is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/url/{WebUtility.UrlEncode(url)}/general", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;

            return await res.Deseralize<URLIndicator>();
        }

        /// <summary>
        /// Get the URLs of a domain/URL prefix.
        /// </summary>
        /// <param name="url">The target domain or URL prefix.</param>
        /// <returns>An instance of <see cref="URLIndicator"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<URLList> GetURLList(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url), "URL is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"indicators/url/{WebUtility.UrlEncode(url)}/url_list");

            return await res.Deseralize<URLList>();
        }
    }
}