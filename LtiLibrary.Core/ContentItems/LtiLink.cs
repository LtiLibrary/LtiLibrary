using LtiLibrary.Core.Common;

namespace LtiLibrary.Core.ContentItems
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
