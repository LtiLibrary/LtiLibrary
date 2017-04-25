using System.Collections.Generic;
using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.ContentItems
{
    /// <summary>
    /// Represents a collection of IMS ContentItems (http://purl.imsglobal.org/ctx/lti/v1/ContentItem)
    /// </summary>
    public class ContentItemSelectionGraph : JsonLdObject
    {
        /// <summary>
        /// Initialize a new instance of the ContentItemSelectionGraph.
        /// </summary>
        public ContentItemSelectionGraph()
        {
            ExternalContextId = LtiConstants.ContentItemContextId;
        }

        /// <summary>
        /// Get or Set the collection of ContentItems.
        /// </summary>
        [JsonProperty("@graph")]
        public IEnumerable<ContentItem> Graph { get; set; }
    }
}
