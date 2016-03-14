using System;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiLibrary.Core.Outcomes.v2
{
    public class LisResult : JsonLdObject
    {
        public LisResult()
        {
            Type = LtiConstants.LisResultType;
        }

        /// <summary>
        /// Optional comment about this Result suitable for display to the learner. 
        /// Typically, this is a comment made by the instructor or grader.
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Optional number of exta credit points earned by the learner.
        /// </summary>
        [JsonProperty("extraCreditScore")]
        public decimal? ExtraCreditScore { get; set; }

        /// <summary>
        /// Optional URI for the person that recorded this Result.
        /// </summary>
        [JsonProperty("gradedBy")]
        public Uri GradedBy { get; set; }

        /// <summary>
        /// Optional score earned by the learner before adding extra credit or subtracting penalties.
        /// </summary>
        [JsonProperty("normalScore")]
        public decimal? NormalScore { get; set; }

        /// <summary>
        /// Optional number of points deducted from the normal score due to some penalty such as submitting an assignment after the due date.
        /// </summary>
        [JsonProperty("penaltyScore")]
        public decimal? PenaltyScore { get; set; }

        /// <summary>
        /// The URI for the person whose score is recorded in this Result.
        /// </summary>
        [JsonProperty("resultAgent")]
        public LisPerson ResultAgent { get; set; }

        /// <summary>
        /// The URI for the LineItem within which this Result is contained.
        /// </summary>
        [JsonProperty("resultOf")]
        public Uri ResultOf { get; set; }

        /// <summary>
        /// Optional final score that should be displayed in a gradebook for this Result object.
        /// </summary>
        [JsonProperty("resultScore")]
        public string ResultScore { get; set; }

        /// <summary>
        /// Optional onstraints on the scores recorded in this Result.
        /// </summary>
        [JsonProperty("resultScoreConstraints")]
        public NumericLimits ResultScoreConstraints { get; set; }

        /// <summary>
        /// Optional status of the result for this user and line item.
        /// </summary>
        [JsonProperty("resultStatus")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultStatus? ResultStatus { get; set; }

        /// <summary>
        /// Optional timestamp.
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Optional total score on the assignment given by totalScore = normalScore + extraCreditScore - penalty.
        /// This value does not take into account the effects of curving. 
        /// </summary>
        [JsonProperty("totalScore")]
        public decimal? TotalScore { get; set; }
    }
}
