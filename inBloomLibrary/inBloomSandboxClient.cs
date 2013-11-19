using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using Microsoft.Web.WebPages.OAuth;
using inBloomLibrary.Models;
using WebMatrix.WebData;

namespace inBloomLibrary
{
    public class inBloomSandboxClient : OAuth2Client
    {
        private const string MyProviderName = "inbloomsandbox";

        public static string Name
        {
            get { return MyProviderName; }
        }

        public inBloomSandboxClient() : base(MyProviderName) { }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            const string authorizationEndpoint = "https://api.sandbox.inbloom.org/api/oauth/authorize";

            var tenantId = HttpContext.Current.Request.QueryString["tenantId"];
            var redirectUrl = new UriBuilder(returnUrl);
            redirectUrl.AppendQueryArgs(new Dictionary<string, string> {
                { "tenantId", tenantId }
            });

            var serviceLoginUrl = new UriBuilder(authorizationEndpoint);
            using (var db = new inBloomLibraryContext())
            {
                var tenant = db.Tenants.SingleOrDefault(t => t.inBloomTenantId == tenantId);
                serviceLoginUrl.AppendQueryArgs(new Dictionary<string, string> {
                    { "client_id", tenant == null ? "" : tenant.ClientId },
                    { "redirect_uri", redirectUrl.Uri.AbsoluteUri }
                });
            }
            return serviceLoginUrl.Uri;
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            const string homeEndpoint = "https://api.sandbox.inbloom.org/api/rest/v1.1/home";
            const string systemCheckEndpoint = "https://api.sandbox.inbloom.org/api/rest/system/session/check";

            var userData = new Dictionary<string, string>();

            // Use /system/session/check to get basic user data
            var request = inBloomApi.CreateWebRequest(systemCheckEndpoint, accessToken);
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var graph = JsonHelper.Deserialize<SessionCheck>(responseStream);
                    userData.AddItemIfNotEmpty("email", graph.Email);
                    userData.AddItemIfNotEmpty("roles", string.Join(",", graph.Roles));
                    userData.AddItemIfNotEmpty("username", graph.ExternalId);
                }
            }

            Link selfLink;

            // Use /home to get the "self" link for the current user
            request = inBloomApi.CreateWebRequest(homeEndpoint, accessToken);
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var graph = JsonHelper.Deserialize<Home>(responseStream);
                    selfLink = graph.Links.SingleOrDefault(l => l.Rel == "self");
                }
            }

            var tenantId = HttpContext.Current.Request.QueryString["tenantId"];

            // Use the "self" link to get the user's id and names
            if (selfLink != null)
            {
                request = inBloomApi.CreateWebRequest(selfLink.Href, accessToken);
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var graph = JsonHelper.Deserialize<Staff>(responseStream);
                        // Store the tenantId with the user id so that we can look
                        // up the long-lived session token to make API calls without
                        // worring about the user token timing out.
                        userData.AddItemIfNotEmpty("id", string.Format("{0}@{1}", graph.Id, tenantId));
                        userData.AddItemIfNotEmpty("firstname", graph.Name.FirstName);
                        userData.AddItemIfNotEmpty("lastname", graph.Name.LastName);
                    }
                }
            }

            return userData;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            const string tokenEndpoint = "https://api.sandbox.inbloom.org/api/oauth/token";

            var tenantId = HttpContext.Current.Request.QueryString["tenantId"];

            var builder = new UriBuilder(tokenEndpoint);
            using (var db = new inBloomLibraryContext())
            {
                var tenant = db.Tenants.SingleOrDefault(t => t.inBloomTenantId == tenantId);
                builder.AppendQueryArgs(
                    new Dictionary<string, string> {
					    { "client_id", tenant == null ? "" : tenant.ClientId },
					    { "redirect_uri", returnUrl.AbsoluteUri },
					    { "client_secret", tenant == null ? "" : tenant.SharedSecret },
					    { "code", authorizationCode },
					    { "grant_type", "authorization_code" },
				    });
            }

            var request = inBloomApi.CreateWebRequest(builder.Uri.AbsoluteUri);
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var graph = JsonHelper.Deserialize<Token>(responseStream);
                    return graph.AccessToken;
                }
            }
        }

        public static inBloomAccount GetCurrentInBloomAccount()
        {
            var providerAccount = OAuthWebSecurity.GetAccountsFromUserName(WebSecurity.CurrentUserName).
                SingleOrDefault(a => a.Provider == MyProviderName);
            if (providerAccount != null)
            {
                return new inBloomAccount(providerAccount.Provider, providerAccount.ProviderUserId);
            }
            return null;
        }
    }
}
