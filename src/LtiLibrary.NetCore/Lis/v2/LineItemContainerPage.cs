﻿using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// Represents an IMS LineItemContainerPage object.
    /// </summary>
    public class LineItemContainerPage : JsonLdObject
    {
        /// <summary>
        /// Initializes a new instance of the LineItemContainerPage class.
        /// </summary>
        public LineItemContainerPage()
        {
            Type = LtiConstants.PageType;
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
