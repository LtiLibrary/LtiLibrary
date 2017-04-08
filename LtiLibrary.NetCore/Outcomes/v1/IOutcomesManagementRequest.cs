namespace LtiLibrary.NetCore.Outcomes.v1
{
    /// <summary>
    /// Represents the Outcomes Management interface introduced in LTI 1.1.
    /// </summary>
    public interface IOutcomesManagementRequest
    {
        string BodyHashReceived { get; set; }
        string LisOutcomeServiceUrl { get; set; }
        string LisResultSourcedId { get; set; }
    }
}
