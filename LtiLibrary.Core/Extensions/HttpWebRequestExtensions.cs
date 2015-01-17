using System.IO;
using System.Net;
using System.Text;

namespace LtiLibrary.Core.Extensions
{
    public static class HttpWebRequestExtensions
    {
        /// <summary>
        /// Create a string representation of the <see cref="HttpWebRequest"/> similar to Fiddler's.
        /// </summary>
        /// <remarks>Created for learning and debugging LTI.</remarks>
        public static string ToFormattedRequestString(this HttpWebRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} HTTP/1.1\n", request.Method, request.RequestUri);
            foreach (var key in request.Headers.AllKeys)
            {
                sb.AppendFormat("{0}: {1}\n", key, string.Join(",", request.Headers.GetValues(key) ?? new string[]{}));
            }
            if (request.ContentLength > 0)
            {
                sb.AppendLine();
                using (var stream = request.GetRequestStream())
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
