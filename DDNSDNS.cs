using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CNET_DDNS
{
    public static class DdnsDns
    {
        public static async Task<string[]> GetCurrentIpForHostName(string hostName)
        {
            var responseList = new List<string>();
            try {
                var ipList = await Dns.GetHostAddressesAsync(hostName);
                if (ipList != null && ipList.Length > 0) {
                    responseList.Add($"The following IP-addresses belong to hostname '{hostName}':");
                    foreach (var ipAddress in ipList) {
                        responseList.Add($"{hostName} > {ipAddress.ToString()}");
                    }
                } else {
                    responseList.Add($"No IP-addresses found for hostname '{hostName}'!");
                }
            } catch (Exception err) {
                responseList.Add($"Error getting IP-addresses for hostname {hostName}: {err.Message}");
            }
            return responseList.ToArray();
        }

    }
}
