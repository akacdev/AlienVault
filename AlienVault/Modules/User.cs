using System;
using System.Net.Http;
using System.Threading.Tasks;
using AlienVault.Entities;

namespace AlienVault.Modules
{
    public class UserModule
    {
        private readonly HttpClient Client;
        private readonly AlienVaultClientConfig Config;

        public UserModule(HttpClient client, AlienVaultClientConfig config)
        {
            Client = client;
            Config = config;
        }

        /// <summary>
        /// Get the current user who the provided API key belongs to. This is useful for testing.
        /// </summary>
        /// <returns>An instance of <see cref="User"/>.</returns>
        public async Task<User> GetCurrentUser()
        {
            HttpResponseMessage res = await Client.Request(HttpMethod.Get, "users/me");
            return await res.Deseralize<User>();
        }

        /// <summary>
        /// Start following a user.
        /// <para>Has no effect if called when already following.</para>
        /// </summary>
        /// <param name="username">The target user's username.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AlienVaultException"></exception>
        public async Task Follow(string username)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username), "Username is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"users/{username}/follow");
            ActionResult action = await res.Deseralize<ActionResult>();

            if (action.Status != "followed") throw new AlienVaultException($"Follow operation ({username}) resulted in an unexpected status: {action.Status}");
        }

        /// <summary>
        /// Stop following a user.
        /// <para>Has no effect if called when already not following.</para>
        /// </summary>
        /// <param name="username">The target user's username.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AlienVaultException"></exception>
        public async Task Unfollow(string username)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username), "Username is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"users/{username}/unfollow");
            ActionResult action = await res.Deseralize<ActionResult>();

            if (action.Status != "unfollowed") throw new AlienVaultException($"Follow operation ({username}) resulted in an unexpected status: {action.Status}");
        }

        /// <summary>
        /// Start subscribing to a user.
        /// <para>Has no effect if called when already subscribed.</para>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AlienVaultException"></exception>
        public async Task Subscribe(string username)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username), "Username is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"users/{username}/subscribe");
            ActionResult action = await res.Deseralize<ActionResult>();

            if (action.Status != "subscribed") throw new AlienVaultException($"Follow operation ({username}) resulted in an unexpected status: {action.Status}");
        }

        /// <summary>
        /// Stop subscribing to a user.
        /// <para>Has no effect if called when already unsubscribed.</para>
        /// </summary>
        /// <param name="username">The target user's username.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AlienVaultException"></exception>
        public async Task Unsubscribe(string username)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username), "Username is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"users/{username}/unsubscribe");
            ActionResult action = await res.Deseralize<ActionResult>();

            if (action.Status != "unsubscribed") throw new AlienVaultException($"Follow operation ({username}) resulted in an unexpected status: {action.Status}");
        }
    }
}