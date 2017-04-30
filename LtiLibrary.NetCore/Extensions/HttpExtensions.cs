using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LtiLibrary.NetCore.Extensions
{
#if DEBUG

    internal static class HttpExtensions
    {
        /// <summary>
        /// Create a string representation of the request similar to Fiddler's.
        /// </summary>
        /// <remarks>Created for learning and debugging LTI.</remarks>
        public static async Task<string> ToFormattedRequestStringAsync(this HttpRequestMessage message,
            HttpContent content = null)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} HTTP/{2}\n", message.Method, message.RequestUri, message.Version);
            foreach (var header in Enumerable.ToList(message.Headers))
            {
                sb.AppendFormat("{0}: {1}\n", header.Key, string.Join(",", header.Value ?? new string[] { }));
            }
            if (content != null && content.Headers.ContentLength > 0)
            {
                sb.AppendLine();
                sb.Append(await content.ReadAsStringAsync());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Create a string representation of the <see cref="HttpResponseMessage"/> similar to Fiddler's.
        /// </summary>
        /// <remarks>Created for learning and debugging LTI.</remarks>
        public static async Task<string> ToFormattedResponseStringAsync(this HttpResponseMessage response)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("HTTP/{0} {1} {2}\n", response.Version, Convert.ToInt32(response.StatusCode),
                response.StatusCode);
            foreach (var header in response.Headers)
            {
                sb.AppendFormat("{0}: {1}\n", header.Key, string.Join(",", header.Value ?? new string[] { }));
            }
            if (response.Content != null && response.Content.Headers.ContentLength > 0)
            {
                sb.AppendLine();
                sb.Append(await response.Content.ReadAsStringAsync());
            }
            return sb.ToString();
        }
    }

#endif
}