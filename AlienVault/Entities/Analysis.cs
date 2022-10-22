using System;
using System.Text.Json.Serialization;

namespace AlienVault.Entities
{
    /// <summary>
    /// Provides sort options for submitted files.
    /// </summary>
    public enum SubmittedFilesSort
    {
        /// <summary>
        /// Sort by the file's add date.
        /// </summary>
        AddDate,
        /// <summary>
        /// Sort by the file's SHA256 hash.
        /// </summary>
        SHA256,
        /// <summary>
        /// Sort by the file's analysis completion date.
        /// </summary>
        CompleteDate
    }

    /// <summary>
    /// Provides sort options for submitted URLs.
    /// </summary>
    public enum SubmittedURLsSort
    {
        /// <summary>
        /// Sort by the URL's add date.
        /// </summary>
        AddDate,
        /// <summary>
        /// Sort by the URL value.
        /// </summary>
        URL,
        /// <summary>
        /// Sort by the URL's analysis completion date.
        /// </summary>
        CompleteDate
    }

    public class StatusBase
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class SubmissionBase : StatusBase
    {
        [JsonPropertyName("result")]
        public string Result { get; set; }
    }

    public class FileSubmission : SubmissionBase
    {
        [JsonPropertyName("sha256")]
        public string SHA256 { get; set; }
    }

    public class URLSubmission : SubmissionBase { }

    public class URLsSubmission : StatusBase
    {
        [JsonPropertyName("updated")]
        public string[] Updated { get; set; }

        [JsonPropertyName("added")]
        public string[] Added { get; set; }

        [JsonPropertyName("exists")]
        public string[] Exists { get; set; }

        [JsonPropertyName("invalid")]
        public string[] Invalid { get; set; }

        [JsonPropertyName("skipped")]
        public string[] Skipped { get; set; }
    }

    public class SubmittedFilesPage
    {
        [JsonPropertyName("results")]
        public SubmittedFile[] Results { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("previous")]
        public string Previous { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }
    }

    public class SubmittedFile
    {
        [JsonPropertyName("user_id")]
        public int AuthorId { get; set; }

        [JsonPropertyName("sha256")]
        public string SHA256 { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("add_date")]
        public DateTime AddDate { get; set; }

        [JsonPropertyName("sent_date")]
        public DateTime? SentDate { get; set; }

        [JsonPropertyName("complete_date")]
        public DateTime? CompleteDate { get; set; }

        [JsonPropertyName("dynamic_complete_date")]
        public DateTime? DynamicCompleteDate { get; set; }

        [JsonPropertyName("tlp")]
        public TLP TLP { get; set; }

        [JsonConverter(typeof(IntBoolConverter))]
        [JsonPropertyName("has_dynamic")]
        public bool HasDynamic { get; set; }

        [JsonConverter(typeof(IntBoolConverter))]
        [JsonPropertyName("previously_existed")]
        public bool PreviouslyExisted { get; set; }
    }

    public class SubmittedURLsPage
    {
        [JsonPropertyName("results")]
        public SubmittedURL[] Results { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("previous")]
        public string Previous { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }
    }

    public class SubmittedURL
    {
        [JsonPropertyName("user_id")]
        public int AuthorId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("add_date")]
        public DateTime AddDate { get; set; }

        [JsonPropertyName("sent_date")]
        public DateTime? SentDate { get; set; }

        [JsonPropertyName("complete_date")]
        public DateTime? CompleteDate { get; set; }

        [JsonPropertyName("unique_hash")]
        public string UniqueHash { get; set; }

        [JsonPropertyName("tlp")]
        public TLP TLP { get; set; }

        [JsonConverter(typeof(IntBoolConverter))]
        [JsonPropertyName("previously_existed")]
        public bool PreviouslyExisted { get; set; }

        [JsonPropertyName("other_user_count")]
        public int OtherUserCount { get; set; }
    }

    public class TLPBase
    {
        [JsonPropertyName("tlp")]
        public TLP TLP { get; set; }
    }

    public class SetFilesTLPParameters : TLPBase
    {
        [JsonPropertyName("hashes")]
        public string[] Hashes { get; set; }
    }

    public class SetURLsTLPParameters : TLPBase
    {
        [JsonPropertyName("urls")]
        public string[] URLs { get; set; }
    }

    public class SetTLPBase
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("updated")]
        public string[] Updated { get; set; }

        [JsonPropertyName("not_permitted")]
        public string[] NotPermitted { get; set; }

        [JsonPropertyName("does_not_exist")]
        public string[] DoesNotExist { get; set; }
    }

    public class SetFilesTLPResult : SetTLPBase { }

    public class SetURLsTLPResult : SetTLPBase { }

    public class URLSubmissionParameters : TLPBase
    {
        [JsonPropertyName("url")]
        public string URL { get; set; }
    }

    public class URLsSubmissionParameters : TLPBase
    {
        [JsonPropertyName("urls")]
        public string[] URLs { get; set; }
    }
}