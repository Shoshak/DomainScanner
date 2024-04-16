using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace DomainScanner
{
    internal class Scanner
    {
        private readonly List<string> DomainList;
        private readonly HttpClient client;

        public Scanner(HttpClient client, List<string> domainList)
        {
            this.client = client;
            DomainList = domainList;
        }

        public IEnumerable<Task<string>> GetDomainsFound(string site)
        {
            List<Uri> urls = new List<Uri>();
            foreach (string domain in DomainList)
            {
                urls.Add(new($"http://{site}.{domain}"));
            }
            IEnumerable<Task<string>> tasks = from url in urls select GetStatus(client, url);
            return tasks;
        }

        private static async Task<string> GetStatus(HttpClient httpClient, Uri link)
        {
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync(link);
                if (response.IsSuccessStatusCode)
                    return link.ToString();
                else return "";
            } catch (Exception)
            {
                return "";
            }
        }
    }
}
