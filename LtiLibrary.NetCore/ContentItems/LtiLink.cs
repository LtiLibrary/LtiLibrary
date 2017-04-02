using LtiLibrary.NetCore.Common;

namespace LtiLibrary.NetCore.ContentItems
{
    public class LtiLink : ContentItem
    {
        public LtiLink()
        {
            Type = LtiConstants.LtiLinkType;
            MediaType = LtiConstants.LtiLinkMediaType;
        }
    }
}
