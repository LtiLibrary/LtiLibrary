using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace LtiLibrary.Core.Extensions
{
    public static class HttpWebResponseExtensions
    {
        /// <summary>
        /// Create a string representation of the <see cref="HttpWebResponse"/> similar to Fiddler's.
        /// </summary>
        /// <remarks>Created for learning and debugging LTI.</remarks>
        public static string ToFormattedResponseString(this HttpResponseMessage response, string body)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("HTTP/1.1 {0} {1}\n", Convert.ToInt32(response.StatusCode), response.StatusCode);
            foreach (var header in response.Headers)
            {
                sb.AppendFormat("{0}: {1}\n", header.Key, string.Join(",", header.Value ?? new string[] { }));
            }
            if (response.Content.Headers.ContentLength > 0)
            {
                sb.AppendLine();
                sb.Append(body);
            }
            return sb.ToString();
        }
    }
}
