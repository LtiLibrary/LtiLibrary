using System.Collections.Generic;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.ContentItems
{
    public class ContentItemSelectionGraph : JsonLdObject
    {
        public ContentItemSelectionGraph() : base(null)
        {
            Context = LtiConstants.ContentItemContext;
        }

        [JsonProperty("@graph")]
        public IEnumerable<ContentItem> Graph { get; set; }
    }
}
