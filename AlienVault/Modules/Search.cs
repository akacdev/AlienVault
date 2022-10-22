using AlienVault.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlienVault.Modules
{
    public class SearchModule
    {
        private readonly HttpClient Client;
        private readonly AlienVaultClientConfig Config;

        public SearchModule(HttpClient client, AlienVaultClientConfig config)
        {
            Client = client;
            Config = config;
        }

        /// <summary>
        /// Search the platform for users.
        /// </summary>
        /// <param name="query">The search query to use.</param>
        /// <param name="sort">The sorting mathod for the results.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="User"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<User[]> Users(string query, UserSort sort = UserSort.Username, int limit = Constants.UserSearchSize * 5)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query), "Search query is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<User> output = new(limit % Constants.UserSearchSize == 0 ? limit : limit + Constants.UserSearchSize);

            int pageLimit = limit < Constants.UserSearchSize ? limit : Constants.UserSearchSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"search/users/?limit={pageLimit}&page={page}&sort={sort.ToString().ToSnakeCase()}&q={query}");

                UserSearchPage chunk = await res.Deseralize<UserSearchPage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Search the platform for pulses.
        /// </summary>
        /// <param name="query">The search query to use.</param>
        /// <param name="sort">The sorting mathod for the results.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Pulse[]> Pulses(string query, PulseSort sort = PulseSort.Modified, int limit = Constants.PulseSearchSize)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query), "Search query is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<Pulse> output = new(limit % Constants.PulseSearchSize == 0 ? limit : limit + Constants.PulseSearchSize);

            int pageLimit = limit < Constants.PulseSearchSize ? Constants.PulseSearchSize - limit : Constants.PulseSearchSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"search/pulses/?limit={pageLimit}&page={page}&sort={sort.ToString().ToSnakeCase()}&q={query}");

                PulseSearchPage chunk = await res.Deseralize<PulseSearchPage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }
    }
}