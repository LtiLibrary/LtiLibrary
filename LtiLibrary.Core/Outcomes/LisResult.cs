using System;
using LtiLibrary.Core.Common;

namespace LtiLibrary.Core.Outcomes
{
    [Obsolete("Use LtiLibrary.Core.Outcomes.v1.LisResult")]
    public class LisResult : BasicResult
    {
        public double? Score { get; set; }
        public string SourcedId { get; set; }
    }
}