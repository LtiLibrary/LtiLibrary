﻿namespace LtiLibrary.NetCore.Lti.v1
{
    /// <summary>
    /// Represents an Outcomes 1.0 result.
    /// </summary>
    public class Result
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