using System;
using System.Net;

namespace CNET_DDNS
{
    public class DdnsWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address) as HttpWebRequest;
            webRequest.UserAgent = "CNET-DDNS-Client";
            webRequest.Timeout = (10 * 1000); // timeout in milliseconds (ms) = 10 sec.
            return webRequest;
        }
    }
}
