# AlienVault 👽

![](https://raw.githubusercontent.com/actually-akac/AlienVault/master/AlienVault/banner.png)

👾 An async C# library for interacting with the AlienVault OTX DirectConnect APIs.

## Usage
Available on NuGet as `AlienVault`, methods can be found under the class `AlienVaultClient`.

Get your API key at: https://otx.alienvault.com/settings

https://www.nuget.org/packages/AlienVault

## Features
- Manage pulses, process events and submit your IOCs
- Made with **.NET 6**
- Fully **async**
- Deep coverage of the API
- Extensive **documentation**
- **No external dependencies** (uses integrated HTTP and JSON)
- **Custom exceptions** (`AlienVaultException`) for advanced catching
- Automatic request retries
- Example project to demonstrate all capabilities of the library

## Example
Under the `Example` directory you can find a working demo project that implements this library.

## Code Samples

### Initializing a new API client
```csharp
AlienVaultClient client = new(new AlienVaultClientConfig()
{
    Key = "cebf8dc104f90bf27153190a24e0fcc31945a5e6fcc1937c5f8640e0fcefc0ec",
    StrictLimit = false
});
```

### Creating a new pulse
```csharp
Pulse pulse = await client.Pulses.Create(new()
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
```

### Adding IOCs to a pulse
```csharp
int revision = await client.Pulses.AddIndicators("622b4458e11410ea56c68052", new IndicatorParameters[]
{
    new()
    {
        Id = 1,
        Type = IndicatorType.Domain,
        Role = IndicatorRole.Unknown,
        Value = "alienvault.com",
        Title = "A sample domain indicator",
        Expiration = DateTime.Now.AddMonths(1)
    }
});
```

### Submitting a file for analysis
```csharp
FileSubmission fileSubmission = await client.Analysis.SubmitFile("C://sample.exe");
```

### Submitting a URL for analysis
```csharp
URLSubmission urlSubmission = await client.Analysis.SubmitURL("http://pzxdvpao.ml/");
```

### Getting general IPv4 information
```csharp
GeneralIPInfo ipv4Info = await client.Data.GetGeneralIPInfo("1.1.1.1");
```

### Getting general domain information
```csharp
GeneralDomainInfo domainInfo = await client.Data.GetGeneralDomainInfo("example.com");
```

### Getting a domain's WHOIS records
```csharp
WhoisEntry[] whoisEntries = await client.Data.GetWhois("t4ck0wsvvpbmktxzluyee11uce27kbct.nl");
```

## Available Methods

You can find all available methods at [/DOCS.md](https://github.com/actually-akac/AlienVault/blob/master/DOCS.md)

## Resources
Website: https://otx.alienvault.com

*This is a community-ran library. Not affiliated with AT&T Cybersecurity.*