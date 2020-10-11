using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace CNET_DDNS
{
    public static class DdnsWeb
    {
        public static async Task<string> MakeRequest(string url, string userName, string password)
        {
            var headers = new NameValueCollection();
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password)) {
                var hash = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{userName}:{password}"));
                headers.Add("Authorization", $"Basic {hash}");
            }
            return await MakeRequest(url, headers);
        }
        
        public static async Task<string> MakeRequest(string url) => await MakeRequest(url, null);

        private static async Task<string> MakeRequest(string url, NameValueCollection headers)
        {
            var result = "";
            try {
                using (var client = new DdnsWebClient()) {
                    if (headers != null) {
                        client.Headers.Add(headers);
                    }
                    result = await client.DownloadStringTaskAsync(url);
                }
            } catch (Exception err) {
                result = $"Error requesting {url}: {err.Message}";
            }
            return result;
        }
    }
}
