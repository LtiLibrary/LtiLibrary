using System;
using System.Linq;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using OAuthLibrary.Models;

namespace OAuthLibrary
{
    public static class OAuthTokens
    {
        public static void SetAccessToken(string providerName, string providerUserId, string token)
        {
            var username = OAuthWebSecurity.GetUserName(providerName, providerUserId);
            var userId = WebSecurity.GetUserId(username);
            using (var db = new OAuthLibraryContext())
            {
                var accessToken = db.AccessTokens.SingleOrDefault(t => t.ProviderName == providerName && t.UserId == userId);
                if (accessToken == null)
                {
                    accessToken = new AccessToken
                    {
                        ProviderName = providerName,
                        UserId = userId
                    };
                    db.AccessTokens.Add(accessToken);
                }
                accessToken.Token = token;
                accessToken.LastUpdated = DateTime.UtcNow;
                db.SaveChanges();
            }
        }

        public static string GetAccessToken(string providerName, string providerUserId)
        {
            var username = OAuthWebSecurity.GetUserName(providerName, providerUserId);
            var userId = WebSecurity.GetUserId(username);
            using (var db = new OAuthLibraryContext())
            {
                var accessToken = db.AccessTokens.SingleOrDefault(t => t.ProviderName == providerName && t.UserId == userId);
                return accessToken == null ? string.Empty : accessToken.Token;
            }
        }
    }
}