using Microsoft.Web.WebPages.OAuth;

namespace inBloomLibrary.Models
{
    public class inBloomAccount : OAuthAccount
    {
        public inBloomAccount(string provider, string providerUserId) : base(provider, providerUserId) 
        {
            if (!string.IsNullOrEmpty(ProviderUserId))
            {
                var index = ProviderUserId.IndexOf('@');
                if (index > 0)
                {
                    SlcUserId = ProviderUserId.Substring(0, index);
                    if (ProviderUserId.Length > index + 1)
                    {
                        TenantId = ProviderUserId.Substring(index + 1);
                    }
                }
            }
        }

        public string SlcUserId { get; private set; }
        public string TenantId { get; private set; }
    }
}
