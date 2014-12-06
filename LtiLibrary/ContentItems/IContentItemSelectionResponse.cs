using LtiLibrary.Lti1;

namespace LtiLibrary.ContentItems
{
    public interface IContentItemSelectionResponse
    {
        string ContentItems { get; set; }
        string Data { get; set; }
        string LtiErrorLog { get; set; }
        string LtiErrorMsg { get; set; }
        string LtiLog { get; set; }
        string LtiMessageType { get; set; }
        string LtiMsg { get; set; }
        string LtiVersion { get; set; }

        LtiRequestViewModel GetLtiRequestViewModel(string consumerSecret);
    }
}