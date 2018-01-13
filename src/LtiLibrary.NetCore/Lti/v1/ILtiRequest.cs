using System;
using System.Collections.Generic;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.OAuth;

namespace LtiLibrary.NetCore.Lti.v1
{
    internal interface ILtiRequest : IOAuthRequest
    {
        string ContextId { get; set; }
        string ContextLabel { get; set; }
        string ContextTitle { get; set; }
        ContextType? ContextType { get; set; }
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
        IList<KeyValuePair<string, string>> Parameters { get; }
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
        IList<Enum> GetRoles();
        void SetCustomParameters(string parameters);
        void SetRoles(IList<Enum> roles);
        string SubstituteCustomVariablesAndGenerateSignature(string consumerSecret);
    }
}
