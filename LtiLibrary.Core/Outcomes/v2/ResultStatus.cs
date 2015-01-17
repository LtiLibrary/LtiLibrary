using LtiLibrary.Core.Common;

namespace LtiLibrary.Core.Outcomes.v2
{
    /// <summary>
    /// ResultStatus instances are enumerable, and they must be referenced by a simple name.
    /// </summary>
    public enum ResultStatus
    {
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/outcomes#Completed")]
        Completed,
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/outcomes#Final")]
        Final,
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/outcomes#Initialized")]
        Initialized,
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/outcomes#Started")]
        Started
    }
}
