using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DomainScanner
{
    internal class Scanner
    {
        private readonly List<string> _domainList;
        private readonly HttpClient _client;

        public Scanner(HttpClient client, List<string> domainList)
        {
            _client = client;
            _domainList = domainList;
        }

        public IEnumerable<Task<string>> GetDomainsFound(string site)
        {
            var urls = _domainList.Select(domain => new Uri($"https://{site}.{domain}")).ToList();
            var tasks = from url in urls select GetStatus(_client, url);
            return tasks;
        }

        private static async Task<string> GetStatus(HttpClient httpClient, Uri link)
        {
            try
            {
                using var response = await httpClient.GetAsync(link);
                return response.IsSuccessStatusCode ? link.ToString() : "";
            } catch (Exception)
            {
                return "";
            }
        }
    }
}
