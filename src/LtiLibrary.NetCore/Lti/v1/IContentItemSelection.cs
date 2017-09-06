namespace LtiLibrary.NetCore.Lti.v1
{
    /// <summary>
    /// LTI Content-Item Message 1.0 ContentItemSelection interface (3 Feb 2015 draft)
    /// </summary>
    internal interface IContentItemSelection
    {
        string ConfirmUrl { get; set; }
        string ContentItems { get; set; }
        string Data { get; set; }
        string LtiErrorLog { get; set; }
        string LtiErrorMsg { get; set; }
        string LtiLog { get; set; }
        string LtiMessageType { get; set; }
        string LtiMsg { get; set; }
        string LtiVersion { get; set; }
    }
}