using System;
using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// A line item is equivalent to a column in a gradebook; it is able to hold the results
    /// associated with a specific activity for a set of users.  The activity is expected to
    /// be associated with a single LTI context within the tool consumer, so there is a 
    /// one-to-many relationship between contexts and line items.
    /// </summary>
    public class LineItem : JsonLdObject
    {
        public LineItem()
        {
            Context = LtiConstants.LineItemContextId;
            Type = LtiConstants.LineItemType;
        }

        /// <summary>
        /// Optional Activity that learners engage with to produce the Results recorded in this LineItem.
        /// </summary>
        [JsonProperty("assignedActivity")]
        public Activity AssignedActivity { get; set; }

        /// <summary>
        /// Optional, human-friendly label for this LineItem suitable for display. 
        /// For example, this label might be used as the heading of a column in a gradebook.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// The context to which this LineItem belongs.
        /// </summary>
        [JsonProperty("lineItemOf")]
        public Context LineItemOf { get; set; }

        /// <summary>
        /// Identifies the property that is reported as the resultScore of the Results within this LineItem.
        /// </summary>
        [JsonProperty("reportingMethod")]
        public string ReportingMethod { get; set; }

        /// <summary>
        /// The container holding the Results for this LineItem.
        /// </summary>
        /// <remarks>
        /// Result is filled in if the requested media type is
        /// application/vnd.ims.lis.v2.lineitemresults+json.
        /// </remarks>
        [JsonProperty("result")]
        public LisResult[] Result { get; set; }

        /// <summary>
        /// Optional URI for the container holding the Results for this LineItem. 
        /// </summary>
        /// <remarks>
        /// Results is filled in if the requested media type is
        /// application/vnd.ims.lis.v2.lineitem+json.
        /// </remarks>
        [JsonProperty("results")]
        public Uri Results { get; set; }

        /// <summary>
        /// Optional constraints on the scores recorded in the Results associated with this LineItem.
        /// </summary>
        [JsonProperty("scoreConstraints")]
        public NumericLimits ScoreContraints { get; set; }
    }
}
