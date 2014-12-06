using System.Collections.Generic;
using LtiLibrary.OAuth;

namespace LtiLibrary.Lti1
{
    public interface ILtiRequest : IOAuthRequest
    {
        string ContextId { get; set; }
        string ContextLabel { get; set; }
        string ContextTitle { get; set; }
        LisContextType? ContextType { get; set; }
        string LaunchPresentationCssUrl { get; set; }
        DocumentTarget? LaunchPresentationDocumentTarget { get; set; }
        int? LaunchPresentationHeight { get; set; }
        string LaunchPresentationLocale { get; set; }
        string LaunchPresentationReturnUrl { get; set; }
        int? LaunchPresentationWidth { get; set; }
        string LisCourseOfferingSourcedId { get; set; }
        string LisCourseSectionSourcedId { get; set; }
        string LisPersonEmailPrimary { get; set; }
        string LisPersonNameFamily { get; set; }
        string LisPersonNameFull { get; set; }
        string LisPersonNameGiven { get; set; }
        string LisPersonSourcedId { get; set; }
        string LtiMessageType { get; set; }
        string LtiVersion { get; set; }
        string Roles { get; set; }
        string ToolConsumerInfoProductFamilyCode { get; set; }
        string ToolConsumerInfoVersion { get; set; }
        string ToolConsumerInstanceContactEmail { get; set; }
        string ToolConsumerInstanceDescription { get; set; }
        string ToolConsumerInstanceGuid { get; set; }
        string ToolConsumerInstanceName { get; set; }
        string ToolConsumerInstanceUrl { get; set; }
        string UserId { get; set; }
        string UserImage { get; set; }

        void AddCustomParameter(string name, string value);
        void AddCustomParameters(string parameters);
        string GenerateSignature(string consumerSecret);
        LtiRequestViewModel GetLtiRequestViewModel(string consumerSecret);
        IList<Role> GetRoles();
        void SetRoles(IList<Role> roles);
    }
}
