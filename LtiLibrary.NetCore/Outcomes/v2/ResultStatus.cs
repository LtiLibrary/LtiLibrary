using LtiLibrary.NetCore.Common;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// ResultStatus instances are enumerable, and they must be referenced by a simple name.
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>
        /// The result is complete.
        /// </summary>
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/outcomes#Completed")]
        Completed,

        /// <summary>
        /// The result is final.
        /// </summary>
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/outcomes#Final")]
        Final,

        /// <summary>
        /// The result is initialized.
        /// </summary>
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/outcomes#Initialized")]
        Initialized,

        /// <summary>
        /// The results is started.
        /// </summary>
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/outcomes#Started")]
        Started
    }
}
