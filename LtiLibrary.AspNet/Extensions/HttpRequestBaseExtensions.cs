using System.Collections.Specialized;
using System.Web;
using LtiLibrary.Core.OAuth;

namespace LtiLibrary.AspNet.Extensions
{
    public static class HttpRequestBaseExtensions
    {
        public static string GenerateOAuthSignature(this HttpRequestBase request, string consumerSecret)
        {
            return OAuthUtility.GenerateSignature(request.HttpMethod, request.Url, request.UnvalidatedParameters(), consumerSecret);
        }

        public static NameValueCollection UnvalidatedParameters(this HttpRequestBase request)
        {
            return new NameValueCollection {request.Unvalidated.QueryString, request.Unvalidated.Form};
        }
    }
}
