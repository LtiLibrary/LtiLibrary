using System;

namespace OAuthLibrary.Models
{
    public class AccessToken
    {
        public int AccessTokenId { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ProviderName { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}