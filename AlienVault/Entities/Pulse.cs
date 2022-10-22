using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlienVault.Entities
{
    /// <summary>
    /// Provides options for TLP levels.
    /// More Info: <a href="https://www.cisa.gov/tlp">https://www.cisa.gov/tlp</a>.
    /// </summary>
    public enum TLP
    {
        White,
        Green,
        Amber,
        Red
    }

    /// <summary>
    /// Provides options for indicator types.
    /// </summary>
    public enum IndicatorType
    {
        IPv4,
        IPv6,
        Domain,
        Hostname,
        Email,
        URL,
        URI,
        FileHashMD5,
        FileHashSHA1,
        FileHashSHA256,
        FileHashPEHASH,
        FileHashIMPHash,
        CIDR,
        FilePath,
        Mutex,
        CVE,
        YARA,
        JA3,
        OSQuery,
        SSLCertFingerprint,
        BitcoinAddress
    }

    /// <summary>
    /// Provides options for indicator roles.
    /// </summary>
    public enum IndicatorRole
    {
        ScanningHost,
        MalwareHosting,
        CommandAndControl,
        ExploitKit,
        Malvertising,
        Phishing,
        Bruteforce,
        WebAttack,
        ExploitSource,
        Trojan,
        RAT,
        Backdoor,
        Adware,
        HackingTool,
        Ransomware,
        Worm,
        MacroMalware,
        DomainOwner,
        DeliveryEmail,
        Unknown,
        FileScanning,
        MemoryScanning,
        Hunting,
        PCAPScanning
    }

    public class PulseBase
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pulse_name")]
        public string PulseName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; }

        [JsonPropertyName("modified")]
        public DateTime? Modified { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        //'Public' is supplied as an integer (1/0). A custom converter is needed to convert these to bools.
        [JsonConverter(typeof(IntBoolConverter))]
        [JsonPropertyName("public")]
        public bool Public { get; set; }

        [JsonPropertyName("adversary")]
        public string Adversary { get; set; }

        [JsonPropertyName("TLP")]
        public TLP TLP { get; set; }

        [JsonPropertyName("cloned_from")]
        public string ClonedFrom { get; set; }

        [JsonPropertyName("locked")]
        public bool Locked { get; set; }

        [JsonPropertyName("pulse_source")]
        public string PulseSource { get; set; }

        [JsonPropertyName("extract_source")]
        public string[] ExtractSource { get; set; }

        [JsonPropertyName("vote")]
        public int Vote { get; set; }

        [JsonPropertyName("author")]
        public Author Author { get; set; }

        [JsonPropertyName("indicator_type_counts")]
        public Dictionary<string, int> Counts { get; set; }

        [JsonPropertyName("is_author")]
        public bool IsAuthor { get; set; }

        [JsonPropertyName("is_subscribing")]
        public bool? IsSubscribing { get; set; }

        [JsonPropertyName("modified_text")]
        public string ModifiedText { get; set; }

        [JsonPropertyName("is_modified")]
        public bool IsModified { get; set; }

        [JsonPropertyName("revision")]
        public int? Revision { get; set; }

        [JsonPropertyName("groups")]
        public Group[] Groups { get; set; }

        [JsonPropertyName("in_group")]
        public bool? InGroup { get; set; }

        [JsonPropertyName("threat_hunter_scannable")]
        public bool ThreatHunterScannable { get; set; }

        //'ThreatHunterHasAgents' is supplied as an integer (1/0). A custom converter is needed to convert these to bools.
        [JsonConverter(typeof(IntBoolConverter))]
        [JsonPropertyName("threat_hunter_has_agents")]
        public bool ThreatHunterHasAgents { get; set; }

        //Counts
        [JsonConverter(typeof(DoubleIntConverter))]
        [JsonPropertyName("export_count")]
        public int? ExportCount { get; set; }

        [JsonConverter(typeof(DoubleIntConverter))]
        [JsonPropertyName("upvotes_count")]
        public int? UpvotesCount { get; set; }

        [JsonConverter(typeof(DoubleIntConverter))]
        [JsonPropertyName("downvotes_count")]
        public int? DownvotesCount { get; set; }

        [JsonConverter(typeof(DoubleIntConverter))]
        [JsonPropertyName("votes_count")]
        public int? VotesCount { get; set; }

        [JsonPropertyName("validator_count")]
        public int? ValidatorCount { get; set; }

        [JsonPropertyName("comment_count")]
        public int? CommentCount { get; set; }

        [JsonPropertyName("follower_count")]
        public int? FollowerCount { get; set; }

        [JsonPropertyName("references_count")]
        public int? ReferencesCount { get; set; }

        [JsonPropertyName("subscriber_count")]
        public int? SubscriberCount { get; set; }

        [JsonPropertyName("tags_count")]
        public int? TagsCount { get; set; }

        [JsonPropertyName("indicator_count")]
        public int? IndicatorCount { get; set; }

        [JsonPropertyName("indicators_count")]
        public int? IndicatorCount2 { set { IndicatorCount = value; } }

        //Arrays
        [JsonPropertyName("upvotes")]
        public string[] Upvotes { get; set; }

        [JsonPropertyName("downvotes")]
        public string[] Downvotes { get; set; }

        [JsonPropertyName("subscribers")]
        public string[] Subscribers { get; set; }

        [JsonPropertyName("followers")]
        public string[] Followers { get; set; }

        [JsonPropertyName("validators")]
        public string[] Validators { get; set; }

        [JsonPropertyName("exported_by")]
        public string[] ExportedBy { get; set; }

        [JsonPropertyName("unsubscribed_users")]
        public string[] UnsubscribedUsers { get; set; }

        [JsonPropertyName("references")]
        public string[] References { get; set; }

        [JsonPropertyName("indicators")]
        public Indicator[] Indicators { get; set; }

        [JsonPropertyName("targeted_countries")]
        public string[] TargetedCountries { get; set; }

        [JsonPropertyName("industries")]
        public string[] Industries { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }
    }

    public class Pulse : PulseBase
    {
        [JsonPropertyName("malware_families")]
        public string[] MalwareFamilies { get; set; }

        [JsonPropertyName("attack_ids")]
        public string[] AttackIds { get; set; }
    }

    public class DetailedPulse : PulseBase
    {
        [JsonPropertyName("malware_families")]
        public MalwareFamily[] MalwareFamilies { get; set; }

        [JsonPropertyName("attack_ids")]
        public AttackId[] AttackIds { get; set; }
    }

    public class AttackId
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
    }

    public class MalwareFamily
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }
    }

    public class Group
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class Indicator
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("pulse_key")]
        public string PulseId { get; set; }

        [JsonPropertyName("indicator")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("created")]
        public DateTime? Created { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("false_positive")]
        public FalsePositive FalsePositive { get; set; }

        [JsonPropertyName("expiration")]
        public DateTime? Expiration { get; set; }

        //'IsActive' is supplied as an integer (1/0). A custom converter is needed to convert these to bools.
        [JsonConverter(typeof(IntBoolConverter))]
        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
    }

    public class FalsePositive
    {
        //accepted
        [JsonPropertyName("assessment")]
        public string Assessment { get; set; }

        [JsonPropertyName("assessment_date")]
        public DateTime? AssessmentDate { get; set; }

        [JsonPropertyName("report_date")]
        public DateTime? ReportDate { get; set; }
    }

    public class IndicatorPage
    {
        [JsonPropertyName("results")]
        public Indicator[] Results { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("previous")]
        public string Previous { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }
    }

    public class PulsePage
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("results")]
        public Pulse[] Results { get; set; }

        [JsonPropertyName("previous")]
        public string Previous { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }
    }

    public class PulseIdPage
    {
        [JsonPropertyName("results")]
        public string[] Results { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("prefetch_pulse_ids")]
        public bool PrefetchPulseIds { get; set; }

        [JsonPropertyName("previous")]
        public object Previous { get; set; }

        [JsonPropertyName("next")]
        public object Next { get; set; }
    }

    public class PulseParameters
    {
        [JsonPropertyName("references")]
        public string[] References { get; set; }

        [JsonPropertyName("indicators")]
        public IndicatorParameters[] Indicators { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("public")]
        public bool Public { get; set; }

        [JsonPropertyName("TLP")]
        public TLP TLP { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }

        [JsonPropertyName("group_ids")]
        public string[] GroupIds { get; set; }

        [JsonPropertyName("industries")]
        public string[] Industries { get; set; }

        [JsonPropertyName("targeted_countries")]
        public string[] TargetedCountries { get; set; }

        [JsonPropertyName("malware_families")]
        public string[] MalwareFamilies { get; set; }

        [JsonPropertyName("attack_ids")]
        public string[] AttackIds { get; set; }

        [JsonPropertyName("adversary")]
        public string Adversary { get; set; }
    }

    public class IndicatorParameters
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("type")]
        public IndicatorType Type { get; set; }

        [JsonPropertyName("role")]
        public IndicatorRole Role { get; set; }

        [JsonPropertyName("indicator")]
        public string Value { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("expiration")]
        public DateTime? Expiration { get; set; }
    }

    public class IndicatorRemoveParameters
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    public class PulseModifyParameters
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonConverter(typeof(IntBoolConverter))]
        [JsonPropertyName("public")]
        public bool? Public { get; set; }

        [JsonPropertyName("TLP")]
        public TLP TLP { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("indicators")]
        public IndicatorModifyParameters Indicators { get; set; } = new();

        [JsonPropertyName("references")]
        public ReferenceModifyParameters References { get; set; } = new();

        [JsonPropertyName("targeted_countries")]
        public TargetedCountriesModifyParameters TargetedCountries { get; set; } = new();
    }

    public class IndicatorModifyParameters
    {
        [JsonPropertyName("add")]
        public IndicatorParameters[] Add { get; set; } = Array.Empty<IndicatorParameters>();

        [JsonPropertyName("edit")]
        public IndicatorParameters[] Edit { get; set; } = Array.Empty<IndicatorParameters>();

        [JsonPropertyName("remove")]
        public IndicatorRemoveParameters[] Remove { get; set; } = Array.Empty<IndicatorRemoveParameters>();
    }

    public class ReferenceModifyParameters
    {
        [JsonPropertyName("add")]
        public string[] Add { get; set; } = Array.Empty<string>();

        [JsonPropertyName("remove")]
        public string[] Remove { get; set; } = Array.Empty<string>();
    }

    public class TargetedCountriesModifyParameters
    {
        [JsonPropertyName("add")]
        public string[] Add { get; set; } = Array.Empty<string>();

        [JsonPropertyName("remove")]
        public string[] Remove { get; set; } = Array.Empty<string>();
    }

    public class ModifiedPulse
    {
        [JsonPropertyName("revision")]
        public int Revision { get; set; }
    }

    public class PulseActionResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("subscriber_count")]
        public int SubscriberCount { get; set; }
    }

    public class EventPage
    {
        [JsonPropertyName("results")]
        public Event[] Results { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("previous")]
        public string Previous { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }
    }

    public class Event
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("object_type")]
        public string ObjectType { get; set; }

        [JsonPropertyName("object_id")]
        public string ObjectId { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }
}