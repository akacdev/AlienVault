### **Analysis**

- public async Task\<FileSubmission> **SubmitFile**(Stream data, string fileName, TLP tlp = TLP.White)
- public async Task\<FileSubmission> **SubmitFile**(string path, TLP tlp = TLP.White)
- public async Task\<SetFilesTLPResult> **SetFilesTLP**(string[] hashes, TLP tlp = TLP.White)
- public async Task\<SetURLsTLPResult> **SetURLsTLP**(string[] urls, TLP tlp = TLP.White)
- public async Task\<SubmittedFile[]> **GetSubmittedFiles**(SubmittedFilesSort sort = SubmittedFilesSort.AddDate, int limit = Constants.SubmittedFilesSize)
- public async Task\<SubmittedURL[]> **GetSubmittedURLs**(SubmittedURLsSort sort = SubmittedURLsSort.AddDate, int limit = Constants.SubmittedURLsSize)
- public async Task\<URLsSubmission> **SubmitURLs**(string[] urls, TLP tlp = TLP.White)
- public async Task\<URLSubmission> **SubmitURL**(string url, TLP tlp = TLP.White)

### **Data**

- public async Task\<AssociatedURL[]> **GetDomainAssociatedURLs**(string domain, int limit = Constants.AssociatedURLsSize)
- public async Task\<AssociatedURL[]> **GetIPAssociatedURLs**(string ip, int limit = Constants.AssociatedURLsSize)
- public async Task\<FileIndicator> **GetFileIndicator**(string hash)
- public async Task\<GeneralDomainInfo> **GetGeneralDomainInfo**(string domain)
- public async Task\<GeneralIPInfo> **GetGeneralIPInfo**(string ip)
- public async Task\<Geolocation> **GetDomainGeo**(string domain)
- public async Task\<Geolocation> **GetIPGeo**(string ip)
- public async Task\<HTTPScan[]> **GetHTTPScans**(string ip)
- public async Task\<Malware[]> **GetDomainMalware**(string domain, int limit = Constants.MalwareSize)
- public async Task\<Malware[]> **GetIPMalware**(string ip, int limit = Constants.MalwareSize)
- public async Task\<PassiveDNS[]> **GetPassiveDomainDNS**(string domain)
- public async Task\<PassiveDNS[]> **GetPassiveIPDNS**(string ip)
- public async Task\<URLIndicator> **GetURLIndicator**(string url)
- public async Task\<URLList> **GetURLList**(string url)
- public async Task\<WhoisEntry[]> **GetWhois**(string domain)

### **Indicator**

- public async Task\<IndicatorTypeInfo[]> **GetTypes**()
- public async Task\<Validation[]> **Validate**(Indicator indicator)

### **Pulse**

- public async Task **Delete**(string id)
- public async Task\<Event[]> **GetEvents**(DateTime? modifiedSince = null, int limit = Constants.EventsSize)
- public async Task\<Indicator[]> **GetIndicators**(string id, int limit = Constants.IndicatorGetSize)
- public async Task\<int> **AddIndicators**(string id, IndicatorParameters[] indicators)
- public async Task\<int> **EditIndicators**(string id, IndicatorParameters[] indicators)
- public async Task\<int> Modify(string id, Pulse**Modify**Parameters parameters)
- public async Task\<int> **RemoveIndicators**(string id, long[] indicatorIds)
- public async Task\<int> **Subscribe**(string id)
- public async Task\<int> **Unsubscribe**(string id)
- public async Task\<Pulse[]> **GetActivityFeed**(DateTime? modifiedSince = null, int limit = Constants.ActivityPulsesSize)
- public async Task\<Pulse[]> **GetAdvancedRelated**( string id = null, string malwareFamily = null, string adversary = null, int limit = Constants.RelatedPulsesSize)
- public async Task\<Pulse[]> **GetMyPulses**(DateTime? modifiedSince = null, int limit = Constants.PulseFeedSize)
- public async Task\<Pulse[]> **GetPulseFeed**(string username, DateTime? modifiedSince = null, int limit = Constants.PulseFeedSize)
- public async Task\<Pulse[]> **GetRelated**(string id, int limit = Constants.RelatedPulsesSize)
- public async Task\<Pulse[]> **GetSubscribed**(DateTime? modifiedSince = null, int limit = Constants.SubscribedPulsesSize)
- public async Task\<Pulse> **Create**(PulseParameters parameters)
- public async Task\<Pulse> **Get**(string id)
- public async Task\<string[]> **GetSubscribedIds**(int limit = Constants.SubscribedPulseIdsSize)

### **Search**

- public async Task\<Pulse[]> **Pulses**(string query, PulseSort sort = PulseSort.Modified, int limit = Constants.PulseSearchSize)
- public async Task\<User[]> **Users**(string query, UserSort sort = UserSort.Username, int limit = Constants.UserSearchSize * 5)

### **User**

- public async Task **Follow**(string username)
- public async Task **Subscribe**(string username)
- public async Task **Unfollow**(string username)
- public async Task **Unsubscribe**(string username)
- public async Task\<User> **GetCurrentUser**()