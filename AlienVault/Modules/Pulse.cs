using AlienVault.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlienVault.Modules
{
    public class PulseModule
    {
        private readonly HttpClient Client;
        private readonly AlienVaultClientConfig Config;

        public PulseModule(HttpClient client, AlienVaultClientConfig config)
        {
            Client = client;
            Config = config;
        }

        /// <summary>
        /// Get a pulse by its ID.
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <returns>An instance of <see cref="Pulse"/> or <c>null</c> if not found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Pulse> Get(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"pulses/{id}", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;

            return await res.Deseralize<Pulse>();
        }

        /// <summary>
        /// Get indicators of a pulse.
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="Indicator"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Indicator[]> GetIndicators(string id, int limit = Constants.IndicatorGetSize)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<Indicator> output = new(limit % Constants.IndicatorGetSize == 0 ? limit : limit + Constants.IndicatorGetSize);

            int pageLimit = limit < Constants.IndicatorGetSize ? limit : Constants.IndicatorGetSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"pulses/{id}/indicators?limit={pageLimit}&page={page}");

                IndicatorPage chunk = await res.Deseralize<IndicatorPage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get pulses related to the provided pulse's ID. Pulses are considered related when they share indicators. 
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="Pulse"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Pulse[]> GetRelated(string id, int limit = Constants.RelatedPulsesSize)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<Pulse> output = new(limit % Constants.RelatedPulsesSize == 0 ? limit : limit + Constants.RelatedPulsesSize);

            int pageLimit = limit < Constants.RelatedPulsesSize ? limit : Constants.RelatedPulsesSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"pulses/{id}/related?limit={pageLimit}&page={page}");

                PulsePage chunk = await res.Deseralize<PulsePage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Provides an advanced method for searching for related pulses.
        /// <para>Ths method will only accept one of: <c>ID</c>, <c>Malware Family</c>, or <c>Adversary</c>.</para>
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <param name="malwareFamily">The malware family to search for.</param>
        /// <param name="adversary">The adversary to search for.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="Pulse"/>.</returns>
        /// <exception cref="AlienVaultException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Pulse[]> GetAdvancedRelated(
            string id = null,
            string malwareFamily = null,
            string adversary = null,
            int limit = Constants.RelatedPulsesSize)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            int specified = id is not null ? 1 : 0 + malwareFamily is not null ? 1 : 0 + adversary is not null ? 1 : 0;
            if (specified > 1) throw new AlienVaultException($"For an advanced related pulses search, only one parameter must be supplied. Your amount: {specified}");

            List<Pulse> output = new(limit % Constants.RelatedPulsesSize == 0 ? limit : limit + Constants.RelatedPulsesSize);

            int pageLimit = limit < Constants.RelatedPulsesSize ? limit : Constants.RelatedPulsesSize;
            int page = 1;
            bool keep = true;

            string path = $"pulses/{(id is null ? "" : $"{id}/")}related?limit={pageLimit}&page={page}&" +
                          (id is null ? "" : $"pulse_id={id}") +
                          (malwareFamily is null ? "" : $"malware_family={WebUtility.UrlEncode(malwareFamily)}") +
                          (adversary is null ? "" : $"adversary={WebUtility.UrlEncode(adversary)}");

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get, path);

                PulsePage chunk = await res.Deseralize<PulsePage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get pulses that you're subscribed to.
        /// </summary>
        /// <param name="modifiedSince">Only return pulses that have been modified since the provided <see cref="DateTime"/>.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Pulse[]> GetSubscribed(DateTime? modifiedSince = null, int limit = Constants.SubscribedPulsesSize)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<Pulse> output = new(limit % Constants.SubscribedPulsesSize == 0 ? limit : limit + Constants.SubscribedPulsesSize);

            int pageLimit = limit < Constants.SubscribedPulsesSize ? limit : Constants.SubscribedPulsesSize;
            int page = 1;
            bool keep = true;

            string modifiedSinceValue = modifiedSince.HasValue ? $"&modified_since={WebUtility.UrlEncode(DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture))}" : "";

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"pulses/subscribed?limit={pageLimit}&page={page}{modifiedSinceValue}");

                PulsePage chunk = await res.Deseralize<PulsePage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get IDs of pulses that you're subscribed to.
        /// </summary>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="string"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<string[]> GetSubscribedIds(int limit = Constants.SubscribedPulseIdsSize)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<string> output = new(limit % Constants.SubscribedPulseIdsSize == 0 ? limit : limit + Constants.SubscribedPulseIdsSize);

            int pageLimit = limit < Constants.SubscribedPulseIdsSize ? limit : Constants.SubscribedPulseIdsSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"pulses/subscribed_pulse_ids?limit={pageLimit}&page={page}");

                PulseIdPage chunk = await res.Deseralize<PulseIdPage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get your activity feed consisting of suggested pulses.
        /// </summary>
        /// <param name="modifiedSince">Only return pulses that have been modified since the provided <see cref="DateTime"/>.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="Pulse"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Pulse[]> GetActivityFeed(DateTime? modifiedSince = null, int limit = Constants.ActivityPulsesSize)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<Pulse> output = new(limit % Constants.ActivityPulsesSize == 0 ? limit : limit + Constants.ActivityPulsesSize);

            int pageLimit = limit < Constants.ActivityPulsesSize ? limit : Constants.ActivityPulsesSize;
            int page = 1;
            bool keep = true;

            string modifiedSinceValue = modifiedSince.HasValue ? $"&modified_since={WebUtility.UrlEncode(DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture))}" : "";

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"pulses/activity?limit={pageLimit}&page={page}{modifiedSinceValue}");

                PulsePage chunk = await res.Deseralize<PulsePage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Create a new pulse under your account.
        /// </summary>
        /// <param name="parameters">Allows you to provide initial pulse properties.</param>
        /// <returns>An instance of <see cref="Pulse"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Pulse> Create(PulseParameters parameters)
        {
            if (parameters is null) throw new ArgumentNullException(nameof(parameters), "Pulse creation parameters are null.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "pulses/create", parameters, HttpStatusCode.Created, Constants.IndicatorOptions);
            return await res.Deseralize<Pulse>();
        }

        /// <summary>
        /// Modify an existing pulse under your account. For adding/removing indicators, I suggest you to use the helper method <see cref="AddIndicators(string, IndicatorParameters[])"/>.
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <param name="parameters">The modifications to make, defined properties will overwrite existing ones.</param>
        /// <returns>An <see cref="int"/> representing the revision number.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> Modify(string id, PulseModifyParameters parameters)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");
            if (parameters is null) throw new ArgumentNullException(nameof(parameters), "Pulse creation parameters are null.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Patch, $"pulses/{id}", parameters, HttpStatusCode.OK, Constants.IndicatorOptions);
            return (await res.Deseralize<ModifiedPulse>()).Revision;
        }

        /// <summary>
        /// Add new indicators to your pulse.
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <param name="indicators">The indicators to add.</param>
        /// <returns>An <see cref="int"/> representing the revision number.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> AddIndicators(string id, IndicatorParameters[] indicators)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");
            if (indicators is null) throw new ArgumentNullException(nameof(indicators), "Indicator parameters are null.");

            return await Modify(id, new()
            {
                Indicators = new()
                {
                    Add = indicators
                }
            });
        }

        /// <summary>
        /// Edit existing indicators in your pulse. To properly edit indicators, you must pass custom IDs to your indicators when creating and editing.
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <param name="indicators">The indicators to modify.</param>
        /// <returns>An <see cref="int"/> representing the revision number.</returns>
        /// <exception cref="AlienVaultException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> EditIndicators(string id, IndicatorParameters[] indicators)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");
            if (indicators is null) throw new ArgumentNullException(nameof(indicators), "Indicator parameters are null.");

            foreach (IndicatorParameters indicator in indicators)
            {
                if (!indicator.Id.HasValue) throw new AlienVaultException($"Indicator with value '{indicator.Value}' is missing an ID, unable to edit.");
            }

            return await Modify(id, new()
            {
                Indicators = new()
                {
                    Edit = indicators
                }
            });
        }

        /// <summary>
        /// Remove existing indicators from your pulse. To properly remove indicators, you must pass custom IDs to your indicators when creating.
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <param name="indicatorIds">The IDs of indicators to remove.</param>
        /// <returns>An <see cref="int"/> representing the revision number.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> RemoveIndicators(string id, long[] indicatorIds)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");
            if (indicatorIds is null) throw new ArgumentNullException(nameof(indicatorIds), "Indicator IDs are null.");

            return await Modify(id, new()
            {
                Indicators = new()
                {
                    Remove = indicatorIds.Select(id => new IndicatorRemoveParameters() { Id = id }).ToArray()
                }
            });
        }

        /// <summary>
        /// Delete a pulse off of your account.
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <returns></returns>
        /// <exception cref="AlienVaultException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"pulses/{id}/delete", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) throw new AlienVaultException($"Provided pulse ID '{id}' doesn't exist.");

            string status = (await res.Deseralize<ActionResult>()).Status;

            if (!status.Contains("Pulse removed") || !status.Contains(id)) throw new AlienVaultException($"Received a success status code when deleting pulse {id}, but response had an unexpected status message: {status}.");
        }

        /// <summary>
        /// Subscribe to a pulse.
        /// <para>This has no effect when already subscribed.</para>
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <returns>The amount of users subscribed to this pulse.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AlienVaultException"></exception>
        public async Task<int> Subscribe(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"pulses/{id}/subscribe", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) throw new AlienVaultException($"Provided pulse ID '{id}' doesn't exist.");

            PulseActionResult result = await res.Deseralize<PulseActionResult>();

            if (result.Status != "subscribed") throw new AlienVaultException($"Received a success status code when subscribing to pulse {id}, but response had an unexpected status message: {result.Status}.");

            return result.SubscriberCount;
        }

        /// <summary>
        /// Unsubscribe from a pulse.
        /// <para>This has no effect when already unsubscribed.</para>
        /// </summary>
        /// <param name="id">The target pulse's ID.</param>
        /// <returns>The amount of users subscribed to this pulse.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AlienVaultException"></exception>
        public async Task<int> Unsubscribe(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "Pulse ID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"pulses/{id}/unsubscribe", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) throw new AlienVaultException($"Provided pulse ID '{id}' doesn't exist.");

            PulseActionResult result = await res.Deseralize<PulseActionResult>();

            if (result.Status != "unsubscribed") throw new AlienVaultException($"Received a success status code when unsubscribing from pulse {id}, but response had an unexpected status message: {result.Status}.");

            return result.SubscriberCount;
        }

        /// <summary>
        /// Get events related to you.
        /// </summary>
        /// <param name="modifiedSince">Only return events since the provided <see cref="DateTime"/>.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="Event"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Event[]> GetEvents(DateTime? modifiedSince = null, int limit = Constants.EventsSize)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<Event> output = new(limit % Constants.EventsSize == 0 ? limit : limit + Constants.EventsSize);

            int pageLimit = limit < Constants.EventsSize ? limit : Constants.EventsSize;
            int page = 1;
            bool keep = true;

            string modifiedSinceValue = modifiedSince.HasValue ? $"&modified_since={WebUtility.UrlEncode(DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture))}" : "";

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"pulses/events?limit={pageLimit}&page={page}{modifiedSinceValue}");

                EventPage chunk = await res.Deseralize<EventPage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get a feed of pulses from a specific user.
        /// </summary>
        /// <param name="username">The target user's username.</param>
        /// <param name="modifiedSince">Only return pulses that have been modified since the provided <see cref="DateTime"/>.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="Pulse"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Pulse[]> GetPulseFeed(string username, DateTime? modifiedSince = null, int limit = Constants.PulseFeedSize)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username), "Username is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<Pulse> output = new(limit % Constants.PulseFeedSize == 0 ? limit : limit + Constants.PulseFeedSize);

            int pageLimit = limit < Constants.PulseFeedSize ? limit : Constants.PulseFeedSize;
            int page = 1;
            bool keep = true;

            string modifiedSinceValue = modifiedSince.HasValue ? $"&modified_since={WebUtility.UrlEncode(DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture))}" : "";

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"pulses/user/{username}?limit={pageLimit}&page={page}{modifiedSinceValue}");

                PulsePage chunk = await res.Deseralize<PulsePage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Get pulses on your account.
        /// </summary>
        /// <param name="modifiedSince">Only return pulses that have been modified since the provided <see cref="DateTime"/>.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="Pulse"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Pulse[]> GetMyPulses(DateTime? modifiedSince = null, int limit = Constants.PulseFeedSize)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<Pulse> output = new(limit % Constants.PulseFeedSize == 0 ? limit : limit + Constants.PulseFeedSize);

            int pageLimit = limit < Constants.PulseFeedSize ? limit : Constants.PulseFeedSize;
            int page = 1;
            bool keep = true;

            string sinceValue = modifiedSince.HasValue ? $"&since={WebUtility.UrlEncode(DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture))}" : "";

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"pulses/my?limit={pageLimit}&page={page}{sinceValue}");

                PulsePage chunk = await res.Deseralize<PulsePage>();
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