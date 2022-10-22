using AlienVault.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlienVault.Modules
{
    public class AnalysisModule
    {
        private readonly HttpClient Client;
        private readonly AlienVaultClientConfig Config;

        public AnalysisModule(HttpClient client, AlienVaultClientConfig config)
        {
            Client = client;
            Config = config;
        }

        /// <summary>
        /// Submit a malicious file from your system for analysis.
        /// </summary>
        /// <param name="path">The path to load the file from.</param>
        /// <param name="tlp">The TLP level of this submission.</param>
        /// <returns>An instance of <see cref="FileSubmission"/>.</returns>
        public async Task<FileSubmission> SubmitFile(string path, TLP tlp = TLP.White)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path), "Path is null or empty.");

            FileInfo info = new(path);
            return await SubmitFile(File.Open(path, FileMode.Open), info.Name, tlp);
        }

        /// <summary>
        /// Submit a malicious file for analysis.
        /// </summary>
        /// <param name="data">The data stream with the file data to upload.</param>
        /// <param name="fileName">The name of the file to analyse.</param>
        /// <param name="tlp">The TLP level of this submission.</param>
        /// <returns>An instance of <see cref="FileSubmission"/>.</returns>
        public async Task<FileSubmission> SubmitFile(Stream data, string fileName, TLP tlp = TLP.White)
        {
            MultipartFormDataContent content = new()
            {
                { new StreamContent(data), "file", fileName },
                { new StringContent(tlp.ToString().ToSnakeCase()), "tlp" }
            };

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "indicators/submit_file", content);
            return await res.Deseralize<FileSubmission>();
        }

        /// <summary>
        /// Get previously submitted files from your account.
        /// </summary>
        /// <param name="sort">The sorting mathod for the results.</param>
        /// <param name="limit">The maximum amount of results to return.</param>
        /// <returns>An array of <see cref="SubmittedFile"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<SubmittedFile[]> GetSubmittedFiles(SubmittedFilesSort sort = SubmittedFilesSort.AddDate, int limit = Constants.SubmittedFilesSize)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<SubmittedFile> output = new(limit % Constants.SubmittedFilesSize == 0 ? limit : limit + Constants.SubmittedFilesSize);

            int pageLimit = limit < Constants.SubmittedFilesSize ? limit : Constants.SubmittedFilesSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"indicators/submitted_files?limit={pageLimit}&page={page}&sort={sort.ToString().ToSnakeCase()}");

                SubmittedFilesPage chunk = await res.Deseralize<SubmittedFilesPage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Set the TLP level for already submitted files.
        /// </summary>
        /// <param name="hashes">The hashes of the target files.</param>
        /// <param name="tlp">The target TLP level.</param>
        /// <returns>An instance of <see cref="SetFilesTLPResult"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AlienVaultException"></exception>
        public async Task<SetFilesTLPResult> SetFilesTLP(string[] hashes, TLP tlp = TLP.White)
        {
            if (hashes is null) throw new ArgumentNullException(nameof(hashes), "Hashes are null.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "indicators/update_submitted_files_tlp", new SetFilesTLPParameters()
            {
                Hashes = hashes,
                TLP = tlp
            });
            SetFilesTLPResult result = await res.Deseralize<SetFilesTLPResult>();

            if (result.Status != "ok") throw new AlienVaultException($"Received a success status code when setting files TLP, but response had an unexpected status message: {result.Status}.");

            return result;
        }

        /// <summary>
        /// Submit a malicious URL.
        /// </summary>
        /// <param name="url">The target URL.</param>
        /// <param name="tlp">The TLP level of this submission.</param>
        /// <returns>An instance of <see cref="URLSubmission"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<URLSubmission> SubmitURL(string url, TLP tlp = TLP.White)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url), "URL is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "indicators/submit_url", new URLSubmissionParameters()
            {
                URL = url,
                TLP = tlp
            });

            return await res.Deseralize<URLSubmission>();
        }

        /// <summary>
        /// Submit multiple malicious URLs at once.
        /// </summary>
        /// <param name="urls">The target URLs.</param>
        /// <param name="tlp">The TLP level of this submission.</param>
        /// <returns>An instance of <see cref="URLsSubmission"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<URLsSubmission> SubmitURLs(string[] urls, TLP tlp = TLP.White)
        {
            if (urls is null) throw new ArgumentNullException(nameof(urls), "URLs are null.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "indicators/submit_urls", new URLsSubmissionParameters()
            {
                URLs = urls,
                TLP = tlp
            });

            return await res.Deseralize<URLsSubmission>();
        }

        /// <summary>
        /// Get previously submitted malicious URLs on your account.
        /// </summary>
        /// <param name="sort">An optional sort method.</param>
        /// <param name="limit"></param>
        /// <returns>An array of <see cref="SubmittedURL"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<SubmittedURL[]> GetSubmittedURLs(SubmittedURLsSort sort = SubmittedURLsSort.AddDate, int limit = Constants.SubmittedURLsSize)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive number.");

            List<SubmittedURL> output = new(limit % Constants.SubmittedURLsSize == 0 ? limit : limit + Constants.SubmittedURLsSize);

            int pageLimit = limit < Constants.SubmittedURLsSize ? limit : Constants.SubmittedURLsSize;
            int page = 1;
            bool keep = true;

            while (keep)
            {
                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"indicators/submitted_urls?limit={pageLimit}&page={page}&sort={sort.ToString().ToSnakeCase()}");

                SubmittedURLsPage chunk = await res.Deseralize<SubmittedURLsPage>();
                output.AddRange(chunk.Results);

                if (chunk.Next is null) keep = false;
                if (output.Count >= limit) keep = false;

                page++;
            }

            if (Config.StrictLimit) output = output.Take(limit).ToList();
            return output.ToArray();
        }

        /// <summary>
        /// Set the TLP level for already submitted URLs.
        /// </summary>
        /// <param name="urls">The target URLs to update.</param>
        /// <param name="tlp">The target TLP level.</param>
        /// <returns>An instance of <see cref="SetURLsTLPResult"/>.</returns>
        /// <exception cref="AlienVaultException"></exception>
        public async Task<SetURLsTLPResult> SetURLsTLP(string[] urls, TLP tlp = TLP.White)
        {
            if (urls is null) throw new ArgumentNullException(nameof(urls), "URLs are null.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "indicators/update_submitted_urls_tlp", new SetURLsTLPParameters()
            {
                URLs = urls,
                TLP = tlp
            });
            SetURLsTLPResult result = await res.Deseralize<SetURLsTLPResult>();

            if (result.Status != "ok") throw new AlienVaultException($"Received a success status code when setting URLs TLP, but response had an unexpected status message: {result.Status}.");

            return result;
        }
    }
}