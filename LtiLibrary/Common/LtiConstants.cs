using System;
using System.Collections.Generic;
using LtiLibrary.Lti1;

namespace LtiLibrary.Common
{
    public static class LtiConstants
    {
        // ReSharper disable InconsistentNaming

        // Build a lookup table of role URNs
        static LtiConstants()
        {
            RoleUrns =  new Dictionary<string, Role>(StringComparer.InvariantCultureIgnoreCase);
            var type = typeof(Role);
            foreach (Role role in Enum.GetValues(type))
            {
                var memInfo = type.GetMember(role.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(UrnAttribute), false);
                var urn = ((UrnAttribute)attributes[0]).Urn;
                RoleUrns.Add(urn, role);
            }
        }
        public static readonly Dictionary<string, Role> RoleUrns;

        // LTI
        public const string LtiVersion = "LTI-1p0";
        public const string BasicLaunchLtiMessageType = "basic-lti-launch-request";
        public const string ContentItemSelectionRequestLtiMessageType = "ContentItemSelectionRequest";
        public const string ContentItemSelectionResponseLtiMessageType = "ContentItemSelectionResponse";

        // Message Parameter Names
        public const string AcceptCopyAdviceParameter = "accept_copy_advice";
        public const string AcceptMediaTypesParameter = "accept_media_types";
        public const string AcceptMultipleParameter = "accept_multiple";
        public const string AcceptPresentationDocumentTargetsParameter = "accept_presentation_document_targets";
        public const string AcceptUnsignedParameter = "accept_unsigned";
        public const string ContentItemDataParameter = "data";
        public const string ContentItemPlacementParameter = "content_items";
        public const string ContentItemReturnUrlParameter = "content_item_return_url";
        public const string ContentItemServiceUrlParameter = "content_item_service_url";
        public const string ContentItemTextParameter = "text";
        public const string ContentItemTitleParameter = "title";
        public const string ContextIdParameter = "context_id";
        public const string ContextIdHistoryParameter = "context_id_history";
        public const string ContextLabelParameter = "context_label";
        public const string ContextTitleParameter = "context_title";
        public const string ContextTypeParameter = "context_type";
        public const string LaunchPresentationCssUrlParameter = "launch_presentation_css_url";
        public const string LaunchPresentationDocumentTargetParameter = "launch_presentation_document_target";
        public const string LaunchPresentationHeightParameter = "launch_presentation_height";
        public const string LaunchPresentationLocaleParameter = "launch_presentation_locale";
        public const string LaunchPresentationReturnUrlParameter = "launch_presentation_return_url";
        public const string LaunchPresentationWidthParameter = "launch_presentation_width";
        public const string LisCourseOfferingSourcedIdParameter = "lis_course_offering_sourcedid";
        public const string LisCourseSectionSourceIdParameter = "lis_course_section_sourcedid";
        public const string LisOutcomeServiceUrlParameter = "lis_outcome_service_url";
        public const string LisPersonContactEmailPrimaryParameter = "lis_person_contact_email_primary";
        public const string LisPersonNameFamilyParameter = "lis_person_name_family";
        public const string LisPersonNameFullParameter = "lis_person_name_full";
        public const string LisPersonNameGivenParameter = "lis_person_name_given";
        public const string LisPersonSourcedIdParameter = "lis_person_sourcedid";
        public const string LisResultSourcedIdParameter = "lis_result_sourcedid";
        public const string LtiErrorLogParameter = "lti_errorlog";
        public const string LtiErrorMsgParameter = "lti_errormsg";
        public const string LtiLogParameter = "lti_log";
        public const string LtiMsgParameter = "lti_msg";
        public const string LtiMessageTypeParameter = "lti_message_type";
        public const string LtiVersionParameter = "lti_version";
        public const string ResourceLinkDescriptionParameter = "resource_link_description";
        public const string ResourceLinkIdParameter = "resource_link_id";
        public const string ResourceLinkIdHistoryParameter = "resource_link_id_history";
        public const string ResourceLinkTitleParameter = "resource_link_title";
        public const string RoleScopeMentorParameter = "role_scope_mentor";
        public const string RolesParameter = "roles";
        public const string ToolConsumerInfoProductFamilyCodeParameter = "tool_consumer_info_product_family_code";
        public const string ToolConsumerInfoVersionParameter = "tool_consumer_info_version";
        public const string ToolConsumerInstanceContactEmailParameter = "tool_consumer_instance_contact_email";
        public const string ToolConsumerInstanceDescriptionParameter = "tool_consumer_instance_description";
        public const string ToolConsumerInstanceGuidParameter = "tool_consumer_instance_guid";
        public const string ToolConsumerInstanceNameParameter = "tool_consumer_instance_name";
        public const string ToolConsumerInstanceUrlParameter = "tool_consumer_instance_url";
        public const string UserIdParameter = "user_id";
        public const string UserImageParameter = "user_image";

        // Media types
        public const string ContentItemsMediaType = "application/vnd.ims.lti.v1.contentitemplacement+json";
        public const string LaunchMediaType = "application/vnd.ims.lti.v1.launch+json";
        public const string OutcomeMediaType = "application/vnd.ims.lti.v1.outcome+xml";
        public const string ToolConsumerProfileMediaType = "application/vnd.ims.lti.v2.toolconsumerprofile+json";

        // Contexts
        public const string ContentItemPlacementContext = "http://purl.imsglobal.org/ctx/lti/v1/ContentItemPlacement";
        public const string ToolConsumerProfileContext = "http://purl.imsglobal.org/ctx/lti/v2/ToolConsumerProfile";

        // Coding
        public const string ScoreLanguage = "en";

        // ReSharper restore InconsistentNaming
    }
}
