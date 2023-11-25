using AlienVault.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlienVault
{
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToSnakeCase();
        }
    }

    public class IndicatorNamingPolicy : JsonNamingPolicy
    {
        public string[] IndicatorTypeNames = Enum.GetNames(typeof(IndicatorType));
        public string[] IndicatorRoleNames = Enum.GetNames(typeof(IndicatorRole));

        public static readonly Dictionary<IndicatorType, string> IndicatorTypeMapping = new()
        {
            { IndicatorType.IPv4, "IPv4" },
            { IndicatorType.IPv6, "IPv6" },
            { IndicatorType.URL, "URL" },
            { IndicatorType.URI, "URI" },
            { IndicatorType.FileHashMD5, "FileHash-MD5" },
            { IndicatorType.FileHashSHA1, "FileHash-SHA1" },
            { IndicatorType.FileHashSHA256, "FileHash-SHA256" },
            { IndicatorType.FileHashPEHASH, "FileHash-PEHASH" },
            { IndicatorType.FileHashIMPHash, "FileHash-IMPHASH" },
            { IndicatorType.CIDR, "CIDR" },
            { IndicatorType.FilePath, "FilePath" },
            { IndicatorType.Mutex, "Mutex" },
            { IndicatorType.YARA, "YARA" },
            { IndicatorType.JA3, "JA3" },
            { IndicatorType.OSQuery, "osquery" },
            { IndicatorType.SSLCertFingerprint, "SSLCertFingerprint" },
            { IndicatorType.BitcoinAddress, "BitcoinAddress" },
        };

        public static readonly Dictionary<IndicatorRole, string> IndicatorRoleMapping = new()
        {
            { IndicatorRole.RAT, "rat" },
            { IndicatorRole.PCAPScanning, "pcap_scanning" }
        };

        public override string ConvertName(string name)
        {
            if (IndicatorTypeNames.Contains(name))
            {
                bool exists = IndicatorTypeMapping.TryGetValue(Enum.Parse<IndicatorType>(name), out string translation);
                if (exists) return translation;
            }
            else if (IndicatorRoleNames.Contains(name))
            {
                bool exists = IndicatorRoleMapping.TryGetValue(Enum.Parse<IndicatorRole>(name), out string translation);
                if (exists) return translation;
            }

            return name.ToSnakeCase();
        }
    }

    public class IntBoolConverter : JsonConverter<bool>
    {
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert.ToInt32(value));
        }

        public override bool Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.String => bool.TryParse(reader.GetString(), out bool b) ? b : throw new JsonException(),
                JsonTokenType.Number => reader.TryGetInt32(out int l) ? Convert.ToBoolean(l) : reader.TryGetDouble(out double d) && Convert.ToBoolean(d),
                _ => throw new JsonException(),
            };
        }
    }

    public class DoubleIntConverter : JsonConverter<int>
    {
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert.ToInt32(value));
        }

        public override int Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => (int)reader.GetDouble(),
                _ => throw new JsonException(),
            };
        }
    }

    public class DoubleLongConverter : JsonConverter<long>
    {
        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert.ToInt64(value));
        }

        public override long Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => (long)reader.GetDouble(),
                _ => throw new JsonException(),
            };
        }
    }
}