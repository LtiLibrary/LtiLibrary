using System;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Outcomes.v2
{
    //[DataContract]
    public class LisResult : JsonLdObject
    {
        public LisResult() : base(LtiConstants.LisResultType)
        {
        }

        /// <summary>
        /// Optional comment about this Result suitable for display to the learner. 
        /// Typically, this is a comment made by the instructor or grader.
        /// </summary>
        //[DataMember(Name = "comment")]
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        /// <summary>
        /// Optional number of exta credit points earned by the learner.
        /// </summary>
        //[DataMember(Name = "extraCreditScore")]
        [JsonProperty("extraCreditScore", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? ExtraCreditScore { get; set; }

        /// <summary>
        /// Optional URI for the person that recorded this Result.
        /// </summary>
        //[DataMember(Name = "gradedBy")]
        [JsonProperty("gradedBy", NullValueHandling = NullValueHandling.Ignore)]
        public Uri GradedBy { get; set; }

        /// <summary>
        /// Optional score earned by the learner before adding extra credit or subtracting penalties.
        /// </summary>
        //[DataMember(Name = "normalScore")]
        [JsonProperty("normalScore", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? NormalScore { get; set; }

        /// <summary>
        /// Optional number of points deducted from the normal score due to some penalty such as submitting an assignment after the due date.
        /// </summary>
        //[DataMember(Name = "penaltyScore")]
        [JsonProperty("penaltyScore", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? PenaltyScore { get; set; }

        /// <summary>
        /// The URI for the person whose score is recorded in this Result.
        /// </summary>
        //[DataMember(Name = "resultAgent")]
        [JsonProperty("resultAgent", Required = Required.Always)]
        public Uri ResultAgent { get; set; }

        /// <summary>
        /// The URI for the LineItem within which this Result is contained.
        /// </summary>
        //[DataMember(Name = "resultOf")]
        [JsonProperty("resultOf", Required = Required.Always)]
        public Uri ResultOf { get; set; }

        /// <summary>
        /// Optional final score that should be displayed in a gradebook for this Result object.
        /// </summary>
        //[DataMember(Name = "resultScore")]
        [JsonProperty("resultScore", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultScore { get; set; }

        /// <summary>
        /// Optional onstraints on the scores recorded in this Result.
        /// </summary>
        //[DataMember(Name = "resultScoreConstraints")]
        [JsonProperty("resultScoreConstraints", NullValueHandling = NullValueHandling.Ignore)]
        public NumericLimits ResultScoreConstraints { get; set; }

        /// <summary>
        /// Optional status of the result for this user and line item.
        /// </summary>
        //[DataMember(Name = "resultStatus")]
        [JsonProperty("resultStatus", NullValueHandling = NullValueHandling.Ignore)]
        public ResultStatus? ResultStatus { get; set; }

        /// <summary>
        /// Optional timestamp.
        /// </summary>
        //[DataMember(Name = "timestamp")]
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Optional total score on the assignment given by totalScore = normalScore + extraCreditScore - penalty.
        /// This value does not take into account the effects of curving. 
        /// </summary>
        //[DataMember(Name = "totalScore")]
        [JsonProperty("totalScore", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? TotalScore { get; set; }
    }
}
