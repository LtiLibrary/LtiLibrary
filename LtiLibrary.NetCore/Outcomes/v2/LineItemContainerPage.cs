using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    public class LineItemContainerPage : JsonLdObject
    {
        public LineItemContainerPage()
        {
            Type = LtiConstants.LineItemContainerPageType;
        }

        /// <summary>
        /// URI for the next page. If there is no next page, the NextPage property will be null.
        /// </summary>
        [JsonProperty("nextPage")]
        public string NextPage { get; set; }

        /// <summary>
        /// The LineItems within this page.
        /// </summary>
        [JsonProperty("pageOf")]
        public LineItemContainer LineItemContainer { get; set; }
    }
}
