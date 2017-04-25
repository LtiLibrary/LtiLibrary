namespace LtiLibrary.NetCore.Lti1
{
    internal interface ILaunchRequest
    {
        string ResourceLinkDescription { get; set; }
        string ResourceLinkId { get; set; }
        string ResourceLinkTitle { get; set; }
        string RoleScopeMentor { get; set; }
    }
}
