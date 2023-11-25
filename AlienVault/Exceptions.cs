using System;
using System.Net.Http;
using System.Net;

namespace AlienVault
{
    /// <summary>
    /// An exception specific to AlienVault for advanced catching. When caused by a HTTP request, you can access the exception's properties to get the context.
    /// </summary>
    public class AlienVaultException : Exception
    {
        /// <summary>
        /// The HTTP request method used that triggered this exception.
        /// </summary>
        public HttpMethod Method { get; set; }
        /// <summary>
        /// The HTTP path used that triggered this exception.
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// The HTTP status code used that triggered this exception.
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }

        public AlienVaultException() { }
        public AlienVaultException(string message) : base(message) { }
        public AlienVaultException(string message, HttpResponseMessage res) : base(message)
        {
            Method = res.RequestMessage.Method;
            Path = res.RequestMessage.RequestUri.AbsolutePath;
            StatusCode = res.StatusCode;
        }
    }
}