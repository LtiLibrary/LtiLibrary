using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace LtiLibrary.Core.Extensions
{
    internal static class HttpExtensions
    {
        public static async Task<HttpResponseMessage> GetResponseAsync(this HttpRequestMessage message, bool allowAutoRedirect = false)
        {
            using (var handler = new HttpClientHandler() { AllowAutoRedirect = allowAutoRedirect })
            {
                using (var client = new HttpClient(handler))
                {
                    return await client.SendAsync(message);
                }
            }
        }

        [Obsolete("Use GetResponseAsync instead.")]
        public static HttpResponseMessage GetResponse(this HttpRequestMessage message, bool allowAutoRedirect = false)
        {
            return GetResponseAsync(message, allowAutoRedirect).Result;
        }

        public static Stream GetResponseStream(this HttpResponseMessage message)
        {
            return message.Content.ReadAsStreamAsync().Result;
        }
        
    }
}
