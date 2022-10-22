using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlienVault.Entities
{
    public class GeolocationBase
    {
        [JsonPropertyName("asn")]
        public string ASN { get; set; }

        [JsonPropertyName("city_data")]
        public bool CityData { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("continent_code")]
        public string ContinentCode { get; set; }

        [JsonPropertyName("country_code3")]
        public string CountryCode3 { get; set; }

        [JsonPropertyName("country_code2")]
        public string CountryCode2 { get; set; }

        [JsonPropertyName("subdivision")]
        public string Subdivision { get; set; }

        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("accuracy_radius")]
        public int AccuracyRadius { get; set; }

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; }

        [JsonPropertyName("country_name")]
        public string CountryName { get; set; }

        [JsonPropertyName("dma_code")]
        public int DmaCode { get; set; }

        [JsonPropertyName("charset")]
        public int Charset { get; set; }

        [JsonPropertyName("area_code")]
        public int AreaCode { get; set; }

        [JsonPropertyName("flag_url")]
        public string FlagURL { get; set; }

        [JsonPropertyName("flag_title")]
        public string FlagTitle { get; set; }

        [JsonPropertyName("net_loc")]
        public string NetLoc { get; set; }
    }

    public class Geolocation : GeolocationBase { }

    public class GeneralIPInfo : GeolocationBase
    {
        [JsonPropertyName("indicator")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("type_title")]
        public string TypeTitle { get; set; }

        [JsonPropertyName("whois")]
        public string WhoisURL { get; set; }

        [JsonPropertyName("reputation")]
        public int Reputation { get; set; }

        [JsonPropertyName("base_indicator")]
        public BaseIndicator BaseIndicator { get; set; }

        [JsonPropertyName("pulse_info")]
        public PulseInfo PulseInfo { get; set; }

        [JsonPropertyName("false_positive")]
        public FalsePositive[] FalsePositives { get; set; }

        [JsonPropertyName("validation")]
        public Validation[] Validations { get; set; }

        [JsonPropertyName("sections")]
        public string[] Sections { get; set; }
    }

    public class RelatedEntries
    {
        [JsonPropertyName("adversary")]
        public string[] Adversary { get; set; }

        [JsonPropertyName("malware_families")]
        public string[] MalwareFamilies { get; set; }

        [JsonPropertyName("industries")]
        public string[] Industries { get; set; }
    }

    public class BaseIndicator
    {
        [JsonPropertyName("id")]
        public object Id { get; set; }

        [JsonPropertyName("indicator")]
        public string Indicator { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("access_type")]
        public string AccessType { get; set; }

        [JsonPropertyName("access_reason")]
        public string AccessReason { get; set; }
    }

    public class PulseInfoBase
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("references")]
        public string[] References { get; set; }

        [JsonPropertyName("related")]
        public RelatedData Related { get; set; }
    }

    public class PulseInfo : PulseInfoBase
    {
        [JsonPropertyName("pulses")]
        public Pulse[] Pulses { get; set; }
    }

    public class DetialedPulseInfo : PulseInfoBase
    {
        [JsonPropertyName("pulses")]
        public DetailedPulse[] Pulses { get; set; }
    }

    public class RelatedData
    {
        [JsonPropertyName("alienvault")]
        public RelatedEntries Alienvault { get; set; }

        [JsonPropertyName("other")]
        public RelatedEntries Other { get; set; }
    }

    public class MalwarePage
    {
        [JsonPropertyName("data")]
        public Malware[] Results { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class Malware
    {
        [JsonPropertyName("hash")]
        public string Sha256 { get; set; }

        [JsonPropertyName("detections")]
        public Detections Detections { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }

    public class Detections
    {
        [JsonPropertyName("avast")]
        public string Avast { get; set; }

        [JsonPropertyName("avg")]
        public string AVG { get; set; }

        [JsonPropertyName("clamav")]
        public string ClamAV { get; set; }

        [JsonPropertyName("msdefender")]
        public string MicrosoftDefender { get; set; }
    }

    public class AssociatedURLsPage
    {
        [JsonPropertyName("url_list")]
        public AssociatedURL[] Results { get; set; }

        [JsonPropertyName("page_num")]
        public int PageNum { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("paged")]
        public bool Paged { get; set; }

        [JsonPropertyName("has_next")]
        public bool HasNext { get; set; }

        [JsonPropertyName("full_size")]
        public int FullSize { get; set; }

        [JsonPropertyName("actual_size")]
        public int ActualSize { get; set; }
    }

    public class AssociatedURL
    {
        [JsonPropertyName("url")]
        public string Value { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("encoded")]
        public string Encoded { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("result")]
        public URLAnalysisResult Result { get; set; }

        [JsonPropertyName("httpcode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("checked")]
        public int Checked { get; set; }

        [JsonPropertyName("deep_analysis")]
        public bool? HasDeepAnalysis { get; set; }

        [JsonConverter(typeof(DoubleLongConverter))]
        [JsonPropertyName("secs")]
        public long Unix { get; set; }

        //TODO: Implement
        /*
            [JsonPropertyName("gsb")]
            public object[] Gsb { get; set; }
        */
    }

    public class URLAnalysisResult
    {
        [JsonPropertyName("urlworker")]
        public URLWorker UrlWorker { get; set; }

        //TODO: Implement
        /*
            [JsonPropertyName("safebrowsing")]
            public SafeBrowsing Safebrowsing { get; set; }
        */
    }

    public class URLWorker
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("http_code")]
        public int StatusCode { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("filetype")]
        public string FileType { get; set; }

        [JsonPropertyName("filemagic")]
        public string FileMagic { get; set; }

        [JsonPropertyName("http_response")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonPropertyName("sha256")]
        public string Sha256 { get; set; }

        [JsonPropertyName("md5")]
        public string MD5 { get; set; }

        [JsonPropertyName("has_file_analysis")]
        public bool? HasFileAnalysis { get; set; }
    }

    public class SafeBrowsing
    {
        //TODO: Implement
        /*
            [JsonPropertyName("matches")]
            public object[] Matches { get; set; }
        */
    }

    public class HTTPScan
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class HTTPScanContainer
    {
        [JsonPropertyName("Error")]
        public string Error { get; set; }

        [JsonPropertyName("data")]
        public HTTPScan[] Data { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class PassiveDNS
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("first")]
        public DateTime First { get; set; }

        [JsonPropertyName("last")]
        public DateTime Last { get; set; }

        [JsonPropertyName("record_type")]
        public string RecordType { get; set; }

        [JsonPropertyName("indicator_link")]
        public string IndicatorURL { get; set; }

        [JsonPropertyName("flag_url")]
        public string FlagUrl { get; set; }

        [JsonPropertyName("flag_title")]
        public string FlagTitle { get; set; }

        [JsonPropertyName("asset_type")]
        public string AssetType { get; set; }

        [JsonPropertyName("asn")]
        public string ASN { get; set; }
    }

    public class PassiveDNSContainer
    {
        [JsonPropertyName("passive_dns")]
        public PassiveDNS[] Results { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class WhoisEntriesContainer
    {
        [JsonPropertyName("data")]
        public WhoisEntry[] Results { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("related")]
        public RelatedDomain[] RelatedDomains { get; set; }
    }

    public class WhoisEntry
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class RelatedDomain
    {
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("related")]
        public string Related { get; set; }

        [JsonPropertyName("related_type")]
        public string RelatedType { get; set; }
    }

    public class GeneralDomainInfo : GeolocationBase
    {
        [JsonPropertyName("indicator")]
        public string Value { get; set; }

        [JsonPropertyName("sections")]
        public string[] Sections { get; set; }

        [JsonPropertyName("whois")]
        public string Whois { get; set; }

        [JsonPropertyName("alexa")]
        public string Alexa { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("type_title")]
        public string TypeTitle { get; set; }

        [JsonPropertyName("validation")]
        public Validation[] Validations { get; set; }

        [JsonPropertyName("base_indicator")]
        public BaseIndicator BaseIndicator { get; set; }

        [JsonPropertyName("pulse_info")]
        public DetialedPulseInfo PulseInfo { get; set; }

        [JsonPropertyName("false_positive")]
        public FalsePositive[] FalsePositives { get; set; }
    }

    public class FileIndicator
    {
        [JsonPropertyName("indicator")]
        public string Hash { get; set; }

        [JsonPropertyName("sections")]
        public string[] Sections { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("type_title")]
        public string TypeTitle { get; set; }

        [JsonPropertyName("validation")]
        public Validation[] Validations { get; set; }

        [JsonPropertyName("base_indicator")]
        public BaseIndicator BaseIndicator { get; set; }

        [JsonPropertyName("pulse_info")]
        public PulseInfo PulseInfo { get; set; }

        [JsonPropertyName("false_positive")]
        public FalsePositive[] FalsePositives { get; set; }
    }

    public class URLIndicator
    {
        [JsonPropertyName("indicator")]
        public string Value { get; set; }

        [JsonPropertyName("sections")]
        public string[] Sections { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("type_title")]
        public string TypeTitle { get; set; }

        [JsonPropertyName("validation")]
        public Validation[] Validations { get; set; }

        [JsonPropertyName("base_indicator")]
        public BaseIndicator BaseIndicator { get; set; }

        [JsonPropertyName("pulse_info")]
        public DetialedPulseInfo PulseInfo { get; set; }

        [JsonPropertyName("false_positive")]
        public FalsePositive[] FalsePositives { get; set; }

        [JsonPropertyName("alexa")]
        public string Alexa { get; set; }

        [JsonPropertyName("whois")]
        public string Whois { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }
    }

    public class URLList : GeolocationBase
    {
        [JsonPropertyName("url_list")]
        public AssociatedURL[] AssociatedURLs { get; set; }
    }
}