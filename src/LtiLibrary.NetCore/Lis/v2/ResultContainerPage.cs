using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// Represents an IMS ResultContainerPage object.
    /// </summary>
    public class ResultContainerPage : JsonLdObject
    {
        /// <summary>
        /// Initializes a new instance of the ResultContainerPage class.
        /// </summary>
        public ResultContainerPage()
        {
            Type = LtiConstants.LisResultContainerType;
        }

        /// <summary>
        /// URI for the next page. If there is no next page, the NextPage property will be null.
        /// </summary>
        [JsonProperty("nextPage")]
        public string NextPage { get; set; }

        /// <summary>
        /// The Results within this page.
        /// </summary>
        [JsonProperty("pageOf")]
        public ResultContainer ResultContainer { get; set; }
    }
}
