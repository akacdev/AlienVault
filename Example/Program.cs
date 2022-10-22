using System;
using System.Threading.Tasks;
using System.Linq;
using AlienVault;
using AlienVault.Entities;
using System.IO;
using System.Text;

namespace Example
{
    public static class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Enter your OTX AlienVault API key:");
            string key = Console.ReadLine();

            AlienVaultClient client = new(new AlienVaultClientConfig()
            {
                Key = key,
                StrictLimit = false
            });

            Console.WriteLine("> Getting information about the current account");
            User user = await client.Users.GetCurrentUser();

            Console.WriteLine($"User: {user.Username} (#{user.Id})");
            Console.WriteLine($"Member Since: {user.MemberSince}");
            Console.WriteLine($"Statistics:");
            Console.WriteLine($"\tSubscribers: {user.SubscriberCount}");
            Console.WriteLine($"\tFollowers: {user.FollowerCount}");
            Console.WriteLine($"\tIndicators: {user.IndicatorCount}");
            Console.WriteLine($"\tPulses: {user.PulseCount}");
            Console.WriteLine($"\tAwards: {user.AwardCount}");
            Console.WriteLine();
            
            Console.WriteLine("> Following AlienVault");
            await client.Users.Follow("AlienVault");
            
            Console.WriteLine("> Unfollowing AlienVault");
            await client.Users.Unfollow("AlienVault");

            Console.WriteLine("> Subscribing to AlienVault");
            await client.Users.Subscribe("AlienVault");

            Console.WriteLine("> Unsubscribing from AlienVault");
            await client.Users.Unsubscribe("AlienVault");
            
            Console.WriteLine("> Searching users matching 'AlienVault', sorted by 'username'");
            User[] users = await client.Search.Users("AlienVault", UserSort.Username, 20);
            
            Console.WriteLine($"Fetched {users.Length} users");
            
            Console.WriteLine("> Searching pulses matching 'malware', sorted by 'modified'");
            Pulse[] pulses = await client.Search.Pulses("malware", PulseSort.Modified, 20);
            
            Console.WriteLine($"Fetched {pulses.Length} pulses");

            Console.WriteLine($"> Getting a pulse");
            Pulse pulse = await client.Pulses.Get("6341d1aa0a02a3f6251ab540");

            if (pulse is null) Console.WriteLine($"Target pulse doesn't exist");
            else Console.WriteLine($"Fetched pulse '{pulse.Name}' from '{pulse.AuthorName}' with {pulse.Indicators.Length} indicators");

            Console.WriteLine($"> Getting a pulse's indicators");
            Indicator[] indicators = await client.Pulses.GetIndicators("6341d1aa0a02a3f6251ab540", 100);
            
            Console.WriteLine($"Fetched {indicators.Length} indicators");

            Console.WriteLine($"> Getting related pulses");
            Pulse[] related = await client.Pulses.GetRelated("61d3b380c44ee030dd092a80", 5);

            Console.WriteLine($"Fetched {related.Length} related pulses");

            Console.WriteLine($"> Getting advanced related pulses");

            Console.WriteLine($"> By ID");
            Pulse[] related1 = await client.Pulses.GetAdvancedRelated(id: "61d3b380c44ee030dd092a80");

            Console.WriteLine($"Fetched {related1.Length} pulses related by ID");

            Console.WriteLine($"> By malware family");
            Pulse[] related2 = await client.Pulses.GetAdvancedRelated(malwareFamily: "Virus:Win32/Nabucur");

            Console.WriteLine($"Fetched {related2.Length} pulses related by a malware family");

            Console.WriteLine($"> By adversary");
            Pulse[] related3 = await client.Pulses.GetAdvancedRelated(adversary: "Equation Group");

            Console.WriteLine($"Fetched {related3.Length} pulses related by an adversary");

            Console.WriteLine($"> Getting subscribed pulses");
            Pulse[] subscribed = await client.Pulses.GetSubscribed();

            Console.WriteLine($"Fetched {subscribed.Length} subscribed pulses");

            Console.WriteLine($"> Getting subscribed pulse IDs");
            string[] subscribedIds = await client.Pulses.GetSubscribedIds();

            Console.WriteLine($"Fetched {subscribedIds.Length} subscribed pulse IDs");
            
            Console.WriteLine($"> Getting the activity feed");
            Pulse[] activity = await client.Pulses.GetActivityFeed();

            Console.WriteLine($"Fetched {activity.Length} pulses in the activity feed"); 
            
            Console.WriteLine($"> Creating a test pulse");
            Pulse created = await client.Pulses.Create(new()
            {
                Name = "Testing Pulse",
                Description = "This pulse was created through the C# AlienVault library!",
                Public = true,
                Tags = new string[]
                {
                    "test",
                    "pulse"
                },
                TLP = TLP.White,
                Indicators = new IndicatorParameters[]
                {
                    new()
                    {
                        Id = 0,
                        Type = IndicatorType.IPv4,
                        Role = IndicatorRole.Unknown,
                        Value = "1.1.1.1",
                        Title = "A sample IP address",
                        Expiration = DateTime.Now.AddYears(1)
                    }
                }
            });
            
            Console.WriteLine($"Created a pulse with ID {created.Id}");
            
            Console.WriteLine($"> Modifying the created pulse and adding an indicator");
            int revision1 = await client.Pulses.Modify(created.Id, new()
            {
                Name = "Modified Testing Pulse",
                Description = "This pulse was created and modified through the C# AlienVault library!",
                Indicators = new()
                {
                    Add = new IndicatorParameters[]
                    {
                        new()
                        {
                            Id = 1,
                            Type = IndicatorType.Domain,
                            Role = IndicatorRole.Unknown,
                            Title = "A sample domain indicator",
                            Expiration = DateTime.Now.AddMonths(1)
                        }
                    }
                }
            });

            Console.WriteLine($"Successfully modified, revision number: {revision1}");

            Console.WriteLine($"> Adding an indicator to the pulse through a helper method");
            int revision2 = await client.Pulses.AddIndicators(created.Id, new IndicatorParameters[]
            {
                new()
                {
                    Id = 2,
                    Type = IndicatorType.Domain,
                    Role = IndicatorRole.Unknown,
                    Value = "alienvault.com",
                    Title = "A sample domain indicator",
                    Expiration = DateTime.Now.AddMonths(1)
                }
            });

            Console.WriteLine($"Successfully added an indicator, revision number: {revision2}");

            Console.WriteLine($"> Editing the latest indicator through a helper method");
            int revision3 = await client.Pulses.EditIndicators(created.Id, new IndicatorParameters[]
            {
                new()
                {
                    Id = 2,
                    Type = IndicatorType.Domain,
                    Role = IndicatorRole.Unknown,
                    Value = "alienvault.com",
                    Title = "An edited domain indicator",
                    Expiration = DateTime.Now.AddMonths(1)
                }
            });

            Console.WriteLine($"Successfully edited the indicator, revision number: {revision3}");

            Console.WriteLine($"> Removing the first indicator through a helper method");
            int revision4 = await client.Pulses.RemoveIndicators(created.Id, new long[]
            {
                0
            });

            Console.WriteLine($"Successfully deleted the indicator, revision number: {revision4}");

            Console.WriteLine($"> Deleting the test pulse");
            await client.Pulses.Delete(created.Id);

            Console.WriteLine("Deleted the test pulse");

            string pulseId = "6341d1aa0a02a3f6251ab540";

            Console.WriteLine($"> Subscribing to a pulse");
            int subscriberCount1 = await client.Pulses.Subscribe(pulseId);

            Console.WriteLine($"Subscribed to a pulse, pulse now has {subscriberCount1} subscribers");

            Console.WriteLine($"> Unsubscribing from a pulse");
            int subscriberCount2 = await client.Pulses.Subscribe(pulseId);

            Console.WriteLine($"Unsubscribed from a pulse, pulse now has {subscriberCount2} subscribers");

            Console.WriteLine($"> Getting recent website events");
            Event[] events1 = await client.Pulses.GetEvents();

            Console.WriteLine($"Fetched {events1.Length} website events");

            Console.WriteLine($"> Getting AlienVault's pulse feeds");
            Pulse[] events2 = await client.Pulses.GetPulseFeed("AlienVault");

            Console.WriteLine($"Fetched {events2.Length} pulses");

            Console.WriteLine($"> Getting the current user's pulses");
            Pulse[] myPulses = await client.Pulses.GetMyPulses();

            Console.WriteLine($"Fetched {myPulses.Length} pulses");

            Console.WriteLine($"> Getting all available indicator types");
            IndicatorTypeInfo[] types = await client.Indicators.GetTypes();

            Console.WriteLine($"Fetched {types.Length} types: {string.Join(", ", types.Select(type => type.Name))}");

            Console.WriteLine($"> Validating an indicator");
            Validation[] validations = await client.Indicators.Validate(new()
            {
                Type = "IPv4",
                Value = "1.1.1.1"
            });

            Console.WriteLine($"Successfully validated, item has the following classifications: {string.Join(", ", validations.Select(x => x.Name))}");
            
            Console.WriteLine("> Submitting a locally generated text file");
            MemoryStream stream = new(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
            FileSubmission fileSubmission = await client.Analysis.SubmitFile(stream, "message.txt");

            Console.WriteLine($"File submitted, result: {fileSubmission.Result}, hash: {fileSubmission.SHA256}");
            
            Console.WriteLine("> Getting recently submitted files");
            SubmittedFile[] files = await client.Analysis.GetSubmittedFiles(limit: 5);
            SubmittedFile latestFile = files.LastOrDefault();

            if (latestFile is null) Console.WriteLine("There are no processed submitted files.");
            else Console.WriteLine($"Latest file is submitted by user: {latestFile.AuthorId}, name: {latestFile.FileName}, TLP: {latestFile.TLP}, previously existed: {latestFile.PreviouslyExisted}, has dynamic analysis: {latestFile.HasDynamic}");

            Console.WriteLine("> Setting TLP levels for file hashes");
            SetFilesTLPResult tlpResult1 = await client.Analysis.SetFilesTLP(new string[]
            {
                latestFile.SHA256,
                "ca0248899dfa63cde2602ade93bc671807d67ed38cc5605beab01854f27950f1",
                "b0985542881c5ef8de49eb892f5ec9040db609d5c7c027a6e64ac1ff467a774e"
            }, TLP.Red);

            Console.WriteLine($"Successfully updated TLP for {tlpResult1.Updated.Length} hashes. Not permitted to change {tlpResult1.NotPermitted.Length} hashes, and {tlpResult1.DoesNotExist.Length} hashes don't exist.");
            
            Console.WriteLine("> Submitting a malicious URL");
            URLSubmission urlSubmission = await client.Analysis.SubmitURL("http://pzxdvpao.ml/");

            Console.WriteLine($"URL submitted, result: {urlSubmission.Result}");

            Console.WriteLine("> Submitting multiple malicious URLs");
            URLsSubmission urlsSubmission = await client.Analysis.SubmitURLs(new string[]
            {
                "https://protected-doc.ml/",
                "http://bldkiecd.ga/",
                "http://bldkiecd.ml/"
            }, TLP.White);

            Console.WriteLine($"URLs submitted, {urlsSubmission.Added.Length} URLs were added, {urlsSubmission.Exists.Length} URLs already exist");

            Console.WriteLine("> Getting recently submitted URLs");
            SubmittedURL[] urls = await client.Analysis.GetSubmittedURLs(limit: 5);
            SubmittedURL latestUrl = urls.LastOrDefault();

            if (latestUrl is null) Console.WriteLine("There are no processed submitted files.");
            else Console.WriteLine($"Latest file is submitted by user: {latestUrl.AuthorId}, hash: {latestUrl.UniqueHash}, previously existed: {latestUrl.PreviouslyExisted}");

            Console.WriteLine("> Setting TLP levels for URLs");
            SetURLsTLPResult tlpResult2 = await client.Analysis.SetURLsTLP(new string[]
            {
                latestUrl.Url
            }, TLP.Red);

            Console.WriteLine($"Successfully updated TLP for {tlpResult2.Updated.Length} URLs. Not permitted to change {tlpResult2.NotPermitted.Length} URLs, and {tlpResult2.DoesNotExist.Length} URLs don't exist.");
            

            Console.WriteLine("> Getting general info about IPv4 and IPv6 addresses");
            GeneralIPInfo ipv4Info = await client.Data.GetGeneralIPInfo("1.1.1.1");
            GeneralIPInfo ipv6Info = await client.Data.GetGeneralIPInfo("2400:8901:0:0:f03c:91ff:fee4:af98");

            Console.WriteLine($"Test IPv4 address is belongs to {ipv4Info.ASN} in {ipv4Info.CountryName}");
            Console.WriteLine($"Test IPv6 address is belongs to {ipv6Info.ASN} in {ipv6Info.CountryName}");

            Console.WriteLine("> Getting geolocation data for an IP address");
            Geolocation geo1 = await client.Data.GetIPGeo("8.8.8.8");

            Console.WriteLine($"IP is owned by {geo1.ASN} and is located in {geo1.CountryName} ({geo1.CountryCode}){(geo1.City is null ? "" : $", {geo1.City}")}");
            Console.WriteLine($"Longitude: {geo1.Longitude}, latitude: {geo1.Latitude}");
            Console.WriteLine($"Flag: {geo1.FlagTitle} => https://otx.alienvault.com{geo1.FlagURL}");

            Console.WriteLine("> Getting all malware related to an IP address");
            Malware[] hits1 = await client.Data.GetIPMalware("1.1.1.1");

            Console.WriteLine($"Fetched {hits1.Length} malware samples related to this IP address.");

            Console.WriteLine("> Getting all analysed URLs associated with an IP address");
            AssociatedURL[] associatedURLs1 = await client.Data.GetIPAssociatedURLs("1.1.1.1");

            Console.WriteLine($"Found {associatedURLs1.Length} associated URLs with the following status codes: " +
                string.Join(", ", associatedURLs1.Select(x => x.StatusCode).Distinct()));

            Console.WriteLine($"> Getting passive DNS entries for an IP address");
            PassiveDNS[] ipDns = await client.Data.GetPassiveIPDNS("1.1.1.1");

            Console.WriteLine($"Fetched {ipDns.Length} passive DNS entries for this IP.");

            Console.WriteLine("> Getting HTTP scans for an IP address");
            HTTPScan[] httpScans = await client.Data.GetHTTPScans("8.8.8.8");

            Console.WriteLine($"Fetched {httpScans.Length} HTTP scans with the following keys: {string.Join(", ", httpScans.Select(x => x.Key))}");

            Console.WriteLine("> Getting general info about a domain");
            GeneralDomainInfo domainInfo = await client.Data.GetGeneralDomainInfo("example.com");

            Console.WriteLine($"Example domain has the following validations: {string.Join(", ", domainInfo.Validations.Select(x => x.Name))}");

            Console.WriteLine("> Getting geolocation data for a domain");
            Geolocation geo2 = await client.Data.GetDomainGeo("example.com");

            Console.WriteLine($"IP is owned by {geo2.ASN} and is located in {geo2.CountryName} ({geo2.CountryCode}){(geo2.City is null ? "" : $", {geo2.City}")}");
            Console.WriteLine($"Longitude: {geo2.Longitude}, latitude: {geo2.Latitude}");
            Console.WriteLine($"Flag: {geo2.FlagTitle} => https://otx.alienvault.com{geo2.FlagURL}");

            Console.WriteLine("> Getting all malware related to a domain");
            Malware[] hits2 = await client.Data.GetDomainMalware("example.com");

            Console.WriteLine($"Fetched {hits2.Length} malware samples related to this domain.");

            Console.WriteLine("> Getting all analysed URLs associated with a domain");
            AssociatedURL[] associatedURLs2 = await client.Data.GetDomainAssociatedURLs("example.com");

            Console.WriteLine($"Found {associatedURLs2.Length} associated URLs with the following status codes: " +
                string.Join(", ", associatedURLs2.Select(x => x.StatusCode).Distinct()));

            Console.WriteLine($"> Getting passive DNS entries for a domain");
            PassiveDNS[] domainDns = await client.Data.GetPassiveDomainDNS("example.com");

            Console.WriteLine($"Fetched {domainDns.Length} passive DNS entries for this domain.");

            Console.WriteLine($"> Getting WHOIS data for a domain");
            WhoisEntry[] whoisEntries = await client.Data.GetWhois("t4ck0wsvvpbmktxzluyee11uce27kbct.nl");

            Console.WriteLine($"Fetched {whoisEntries.Length} WHOIS entries: {string.Join(", ", whoisEntries.Select(x => x.Key.Trim()))}");

            Console.WriteLine("> Getting a file indicator");
            FileIndicator fileIndicator = await client.Data.GetFileIndicator("af69bc23d309a81d8ad7221c8e7e261f4e237df0409f42e3eab60cf1662db352");

            Console.WriteLine($"Fetched file indicator, present in {fileIndicator.PulseInfo.Count} pulses");

            Console.WriteLine("> Getting an URL indicator");
            URLIndicator urlIndicator = await client.Data.GetURLIndicator("example.com");

            Console.WriteLine($"Fetched file indicator, present in {urlIndicator.PulseInfo.Count} pulses");

            Console.WriteLine("> Getting a URL list");
            URLList list = await client.Data.GetURLList("example.com");
            AssociatedURL[] associatedURLs = list.AssociatedURLs;

            Console.WriteLine($"Found {associatedURLs.Length} associated URLs");
            
            Console.WriteLine();
            Console.WriteLine("Demo finished");
            Console.ReadKey();
        }
    }
}