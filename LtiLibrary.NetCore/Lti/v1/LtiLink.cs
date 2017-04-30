using LtiLibrary.NetCore.Common;

namespace LtiLibrary.NetCore.Lti.v1
{
    /// <summary>
    /// Represents an IMS LtiLink ContentItem.
    /// </summary>
    public class LtiLink : ContentItem
    {
        /// <summary>
        /// Initialize a new instance of the LtiLink.
        /// </summary>
        public LtiLink()
        {
            Type = LtiConstants.LtiLinkType;
            MediaType = LtiConstants.LtiLtiLinkMediaType;
        }
    }
}
