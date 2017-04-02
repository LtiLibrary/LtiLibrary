using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LtiLibrary.NetCore.Extensions
{
    public static class HttpWebRequestExtensions
    {
        /// <summary>
        /// Create a string representation of the <see cref="HttpRequestMessage"/> similar to Fiddler's.
        /// </summary>
        /// <remarks>Created for learning and debugging LTI.</remarks>
        public static async Task<string> ToFormattedRequestStringAsync(this HttpRequestMessage request)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} HTTP/1.1\n", request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                sb.AppendFormat("{0}: {1}\n", header.Key, string.Join(",", header.Value ?? new string[]{}));
            }
            if (request.Content != null && request.Content.Headers.ContentLength > 0)
            {
                sb.AppendLine();
                sb.Append(await request.Content.ReadAsStringAsync());
            }
            return sb.ToString();
        }
    }
}
