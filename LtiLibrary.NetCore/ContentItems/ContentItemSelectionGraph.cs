using System.Collections.Generic;
using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.ContentItems
{
    public class ContentItemSelectionGraph : JsonLdObject
    {
        public ContentItemSelectionGraph()
        {
            ExternalContextId = LtiConstants.ContentItemContextId;
        }

        [JsonProperty("@graph")]
        public IEnumerable<ContentItem> Graph { get; set; }
    }
}
