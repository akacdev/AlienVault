using AlienVault.Entities;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlienVault.Modules
{
    public class IndicatorModule
    {
        private readonly HttpClient Client;
        private readonly AlienVaultClientConfig Config;

        public IndicatorModule(HttpClient client, AlienVaultClientConfig config)
        {
            Client = client;
            Config = config;
        }

        /// <summary>
        /// Get the types of indicators you can use.
        /// </summary>
        /// <returns>An array of <see cref="IndicatorTypeInfo"/>.</returns>
        public async Task<IndicatorTypeInfo[]> GetTypes()
        {
            HttpResponseMessage res = await Client.Request(HttpMethod.Get, "pulses/indicators/types");
            return (await res.Deseralize<IndicatorTypesContainer>()).Detail;
        }

        /// <summary>
        /// Validate your indicators before uploading them to a pulse. This is useful for checking the tags of various IoCs.
        /// </summary>
        /// <param name="indicator">The indicator to check.</param>
        /// <returns>An array of <see cref="Validation"/>.</returns>
        /// <exception cref="AlienVaultException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Validation[]> Validate(Indicator indicator)
        {
            if (indicator is null) throw new ArgumentNullException(nameof(indicator), "Indicator is null.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "pulses/indicators/validate", indicator);
            IndicatorValidation validation = await res.Deseralize<IndicatorValidation>();

            if (validation.Status != "success") throw new AlienVaultException($"Received a success status code when validating indicators, but response had an unexpected status message: {validation.Status}.");
            if (!string.IsNullOrEmpty(validation.Message)) throw new AlienVaultException($"Received a message when validating indicator: {validation.Message}");

            return validation.Result;
        }
    }
}