using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// Defines the maximum values for numerical scores.
    /// </summary>
    public class NumericLimits : JsonLdObject
    {
        public NumericLimits()
        {
            Type = LtiConstants.NumericLimitsType;
        }

        /// <summary>
        /// The maximum number of extra credit points that a learner may earn.
        /// </summary>
        [JsonProperty("extraCreditMaximum")]
        public float? ExtraCreditMaximum { get; set; } 

        /// <summary>
        /// The maximum number of points that a learner may earn without extra credit.
        /// </summary>
        [JsonProperty("normalMaximum")]
        public float? NormalMaximum { get; set; }

        /// <summary>
        /// The maximum number of points that a learner may earn. This value is given by:
        /// totalMaximum = normalMaximum + extraCreditMaximum
        /// </summary>
        [JsonProperty("totalMaximum")]
        public float? TotalMaximum { get; set; }
    }
}
