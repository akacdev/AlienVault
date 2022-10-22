using System;
using System.Net.Http;

namespace AlienVault
{
    public class AlienVaultException : Exception
    {
        public string Content { get; set; }

        public AlienVaultException() { }
        public AlienVaultException(string message) : base(message) { }
        public AlienVaultException(string message, string content) : base(message) { Content = content; }
    }
}