namespace LtiLibrary.NetCore.Outcomes.v1
{
    /// <summary>
    /// Represents an IMS LisResult.
    /// </summary>
    public class LisResult
    {
        /// <summary>
        /// Get or set the score for this result. Can be null.
        /// </summary>
        public double? Score { get; set; }

        /// <summary>
        /// Get or set the SourcedId for this result.
        /// </summary>
        public string SourcedId { get; set; }
    }
}