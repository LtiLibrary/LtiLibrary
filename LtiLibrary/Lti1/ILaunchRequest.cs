namespace LtiLibrary.Lti1
{
    public interface ILaunchRequest
    {
        string ResourceLinkDescription { get; set; }
        string ResourceLinkId { get; set; }
        string ResourceLinkTitle { get; set; }
        string RoleScopeMentor { get; set; }
    }
}
