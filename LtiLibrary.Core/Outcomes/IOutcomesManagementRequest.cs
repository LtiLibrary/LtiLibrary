namespace LtiLibrary.Core.Outcomes
{
    public interface IOutcomesManagementRequest
    {
        string LisOutcomeServiceUrl { get; set; }
        string LisResultSourcedId { get; set; }
        string ImsxPoxEnvelope { get; set; }
    }
}
