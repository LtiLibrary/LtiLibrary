using System;
using System.Net;
using System.Text;

namespace LtiLibrary.Core.Extensions
{
    public static class HttpWebResponseExtensions
    {
        /// <summary>
        /// Create a string representation of the <see cref="HttpWebResponse"/> similar to Fiddler's.
        /// </summary>
        /// <remarks>Created for learning and debugging LTI.</remarks>
        public static string ToFormattedResponseString(this HttpWebResponse response, string body)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("HTTP/1.1 {0} {1}\n", Convert.ToInt32(response.StatusCode), response.StatusCode);
            foreach (var key in response.Headers.AllKeys)
            {
                sb.AppendFormat("{0}: {1}\n", key, string.Join(",", response.Headers.GetValues(key) ?? new string[] { }));
            }
            if (response.ContentLength > 0)
            {
                sb.AppendLine();
                sb.Append(body);
            }
            return sb.ToString();
        }
    }
}
