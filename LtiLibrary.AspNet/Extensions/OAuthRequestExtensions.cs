using System.Web;
using LtiLibrary.Core.OAuth;

namespace LtiLibrary.AspNet.Extensions
{
    public static class OAuthRequestExtensions
    {
        public static void ParseRequest(this IOAuthRequest oauthRequest, HttpRequestBase request)
        {
            ParseRequest(oauthRequest, request, false);
        }

        public static void ParseRequest(this IOAuthRequest oauthRequest, HttpRequestBase request, bool skipValidation)
        {
            oauthRequest.HttpMethod = request.HttpMethod;
            oauthRequest.Url = request.Url;

            // Launch requests pass parameters as form fields
            oauthRequest.Parameters.Add(skipValidation ? request.Unvalidated.Form : request.Form);
        }
    }
}
