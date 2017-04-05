namespace LtiLibrary.NetCore.Outcomes.v1
{
    /// <summary>
    /// Represents the Outcomes Management interface introduced in LTI 1.1.
    /// </summary>
    public interface IOutcomesManagementRequest
    {
        string LisOutcomeServiceUrl { get; set; }
        string LisResultSourcedId { get; set; }
        string ImsxPoxEnvelope { get; set; }
    }
}
