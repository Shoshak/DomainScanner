using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DomainScanner;

public partial class Scanner(HttpClient client, List<string> domainList)
{
    public IEnumerable<Task<SiteInfo>> GetDomainsFound(string site)
    {
        return domainList
            .Select(domain => new Uri($"https://{site}.{domain}"))
            .Select(Scan);
    }

    private async Task<SiteInfo> Scan(Uri uri)
    {
        var content = await client.GetStringAsync(uri);
        var title = TitleRegex().Match(content).Groups["Title"].Value;
        return new SiteInfo(uri, title);
    }

    [GeneratedRegex(@"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex TitleRegex();
}