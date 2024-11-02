using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DomainScanner;

public class Scanner
{
    private readonly List<string> _domainList;
    private readonly HttpClient _client;

    public Scanner(HttpClient client, List<string> domainList)
    {
        _client = client;
        _domainList = domainList;
    }

    public IEnumerable<Task<SiteInfo>> GetDomainsFound(string site)
    {
        return _domainList
            .Select(domain => new Uri($"https://{site}.{domain}"))
            .Select(Scan);
    }

    private async Task<SiteInfo> Scan(Uri uri)
    {
        var content = await _client.GetStringAsync(uri);
        var title = Regex
            .Match(content, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase)
            .Groups["Title"].Value;
        return new SiteInfo(uri, title);
    }
}