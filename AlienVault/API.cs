using AlienVault.Entities;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlienVault
{
    public static class API
    {
        public const int MaxRetries = 3;
        public const int RetryDelay = 1000 * 1;
        public const int PreviewMaxLength = 500;

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            object obj,
            HttpStatusCode target = HttpStatusCode.OK,
            JsonSerializerOptions options = null)
        => await Request(cl, method, url, new StringContent(JsonSerializer.Serialize(obj, options ?? Constants.EnumOptions), Encoding.UTF8, "application/json"), target);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            HttpContent content = null,
            HttpStatusCode target = HttpStatusCode.OK)
        {
            int retries = 0;

            HttpResponseMessage res = null;
            while (retries < MaxRetries)
            {
                HttpRequestMessage req = new(method, url)
                {
                    Content = content
                };

                res = await cl.SendAsync(req);

                retries++;

                if (res.StatusCode != HttpStatusCode.InternalServerError) break;
                else await Task.Delay(RetryDelay);
            }

            if (retries == MaxRetries) throw new AlienVaultException($"Failed to request {method} {url} {retries} times. Remote endpoint is returning an 'Internal Server Error' message.");

            if (!target.HasFlag(res.StatusCode))
            {
                string prefix = $"Failed to request {method} {url}, ";
                string text = await res.Content.ReadAsStringAsync();

                MediaTypeHeaderValue contentType = res.Content.Headers.ContentType;
                if (contentType is null) throw new AlienVaultException(string.Concat(prefix, "the 'Content-Type' header is missing."));

                bool isJson = contentType.MediaType.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase);

                if (!isJson) throw new AlienVaultException(string.Concat(
                    prefix,
                    $"received status code {res.StatusCode} and Content-Type {contentType.MediaType}",
                    $"\nPreview: {text[..Math.Min(text.Length, PreviewMaxLength)]}"), text);

                ApiError error = await res.Deseralize<ApiError>();
                if (error is null) throw new AlienVaultException(string.Concat(prefix, "parsed error object is a null."));

                bool hasStatus = !string.IsNullOrEmpty(error.Status);
                bool hasDetail = !string.IsNullOrEmpty(error.Detail);

                if (!hasStatus && !hasDetail) throw new AlienVaultException(string.Concat(prefix, "parsed error object is missing necessary properties."));

                throw new AlienVaultException(string.Concat(
                    prefix,
                    "operation resulted in the following API error:",
                    hasStatus ? $"\nStatus: {error.Status}" : "",
                    hasDetail ? $"\nDetail: {error.Detail}" : ""), text);
            }

            return res;
        }

        public static async Task<T> Deseralize<T>(this HttpResponseMessage res)
        {
            string json = await res.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json)) throw new AlienVaultException("Response content is empty, can't parse as JSON.");

            try
            {
                return JsonSerializer.Deserialize<T>(json, Constants.EnumOptions);
            }
            catch (Exception ex)
            {
                throw new AlienVaultException($"Exception while parsing JSON: {ex.GetType().Name} => {ex.Message}\nJSON preview: {json[..Math.Min(json.Length, PreviewMaxLength)]}");
            }
        }
    }
}