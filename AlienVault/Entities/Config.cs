namespace AlienVault.Entities
{
    /// <summary>
    /// Used to set configuration properties when using this library.
    /// </summary>
    public class AlienVaultClientConfig
    {
        /// <summary>
        /// Create a new instance of the configuration class.
        /// </summary>
        public AlienVaultClientConfig() { }

        /// <summary>
        /// Your AlienVault API key. Get one at <a href="https://otx.alienvault.com/settings">https://otx.alienvault.com/settings</a>.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Whether to strictly return the provided maximum amount of results (<c>limit</c>).
        /// <para><b>False</b> by default.</para>
        /// </summary>
        public bool StrictLimit { get; set; }
    }
}