using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlienVault
{
    internal static class Extensions
    {
        public static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        /// <summary>
        /// Deserialize a JSON HTTP response into a given type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="res">The HTTP response message with JSON as a body.</param>
        /// <param name="options">Optional JSON deserialization options object.</param>
        public static async Task<T> Deseralize<T>(this HttpResponseMessage res, JsonSerializerOptions options = null)
        {
            using Stream stream = await res.Content.ReadAsStreamAsync();
            if (stream.Length == 0) throw new AlienVaultException("Response content is empty, can't parse as JSON.", res);

            try
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, options ?? Constants.EnumOptions);
            }
            catch (Exception ex)
            {
                throw new AlienVaultException($"Exception while parsing JSON: {ex.GetType().Name} => {ex.Message}\nPreview: {await stream.GetPreview()}", res);
            }
        }

        /// <summary>
        /// Serialize an object into a JSON HTTP Stream Content.
        /// </summary>
        /// <param name="obj">The object to serialize as JSON.</param>
        /// <param name="options">Optional JSON serialization options object.</param>
        public static async Task<StreamContent> Serialize(this object obj, JsonSerializerOptions options = null)
        {
            MemoryStream ms = new();
            await JsonSerializer.SerializeAsync(ms, obj, options);
            ms.Position = 0;

            StreamContent sc = new(ms);
            sc.Headers.ContentType = new("application/json")
            {
                CharSet = "utf-8"
            };

            return sc;
        }

        /// <summary>
        /// Extract a short preview string from a HTTP response body.
        /// </summary>
        /// <param name="res">The HTTP response message with a body.</param>
        public static async Task<string> GetPreview(this HttpResponseMessage res)
        {
            using Stream stream = await res.Content.ReadAsStreamAsync();
            if (stream.Length == 0) throw new AlienVaultException("Response content is empty, can't extract body.", res);

            return await GetPreview(stream);
        }

        /// <summary>
        /// Extract a short preview string from a HTTP response body.
        /// </summary>
        /// <param name="stream">The HTTP response stream.</param>
        public static async Task<string> GetPreview(this Stream stream)
        {
            stream.Position = 0;
            using StreamReader sr = new(stream);

            char[] buffer = new char[Math.Min(stream.Length, Constants.PreviewMaxLength)];
            int bytesRead = await sr.ReadAsync(buffer, 0, buffer.Length);
            string text = new(buffer, 0, bytesRead);

            return text;
        }
    }
}