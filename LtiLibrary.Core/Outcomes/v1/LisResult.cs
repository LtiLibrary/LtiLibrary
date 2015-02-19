using LtiLibrary.Core.Common;

namespace LtiLibrary.Core.Outcomes.v1
{
    public class LisResult : BasicResult
    {
        public double? Score { get; set; }
        public string SourcedId { get; set; }
    }
}