using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LtiLibrary.Core.Extensions
{
    public static class HttpWebRequestExtensions
    {
        /// <summary>
        /// Create a string representation of the <see cref="HttpWebRequest"/> similar to Fiddler's.
        /// </summary>
        /// <remarks>Created for learning and debugging LTI.</remarks>
        public static async Task<string> ToFormattedRequestString(this HttpRequestMessage request)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} HTTP/1.1\n", request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                sb.AppendFormat("{0}: {1}\n", header.Key, string.Join(",", header.Value ?? new string[]{}));
            }
            if (request.Content.Headers.ContentLength > 0)
            {
                sb.AppendLine();
                using (var stream = await request.Content.ReadAsStreamAsync())
                {
                    if (stream.CanRead)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            sb.Append(reader.ReadToEnd());
                        }
                        if (stream.CanSeek)
                        {
                            stream.Position = 0;
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}
