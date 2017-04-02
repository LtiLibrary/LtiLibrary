using LtiLibrary.NetCore.Common;

namespace LtiLibrary.NetCore.Outcomes.v1
{
    public class LisResult : BasicResult
    {
        public double? Score { get; set; }
        public string SourcedId { get; set; }
    }
}