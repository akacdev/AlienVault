using AlienVault.Entities;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlienVault
{
    internal static class API
    {
        public const int MaxRetries = 3;
        public const int RetryDelay = 1000;

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            object obj,
            HttpStatusCode target = HttpStatusCode.OK,
            JsonSerializerOptions options = null)
        => await Request(cl, method, url, await obj.Serialize(options ?? Constants.EnumOptions), target);

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
                using HttpRequestMessage req = new(method, url)
                {
                    Content = content
                };

                res = await cl.SendAsync(req);

                retries++;

                if (res.StatusCode != HttpStatusCode.InternalServerError) break;
                else await Task.Delay(RetryDelay);
            }

            content?.Dispose();

            if (retries == MaxRetries)
                throw new AlienVaultException($"Failed to request {method} {url} after {retries} attempts. The API is returning an Internal Server Error.");

            if (target.HasFlag(res.StatusCode)) return res;

            string prefix = $"Failed to request {method} {url}, ";

            MediaTypeHeaderValue contentType = res.Content.Headers.ContentType
                ?? throw new AlienVaultException(string.Concat(prefix, "the \"Content-Type\" header is missing."), res);

            bool isJson = contentType.MediaType == "application/json";

            if (!isJson) throw new AlienVaultException(string.Concat(
                prefix,
                $"received status code {res.StatusCode} and Content-Type {contentType.MediaType}",
                $"\nPreview: {await res.GetPreview()}"));

            ApiError error = await res.Deseralize<ApiError>()
                ?? throw new AlienVaultException(string.Concat(prefix, "failed to parse the error object"), res);

            bool hasStatus = !string.IsNullOrEmpty(error.Status);
            bool hasDetail = !string.IsNullOrEmpty(error.Detail);

            if (!hasStatus && !hasDetail) throw new AlienVaultException(string.Concat(prefix, "parsed error object is missing necessary properties."));

            throw new AlienVaultException(string.Concat(
                prefix,
                "operation resulted in the following API error:",
                hasStatus ? $"\nStatus: {error.Status}" : "",
                hasDetail ? $"\nDetail: {error.Detail}" : ""));
        }
    }
}