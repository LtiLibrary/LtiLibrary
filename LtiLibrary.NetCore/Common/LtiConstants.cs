using System;
using System.Collections.Generic;
using System.Reflection;
using LtiLibrary.NetCore.Lti1;
using System.Linq;

namespace LtiLibrary.NetCore.Common
{
    /// <summary>
    /// LTI constants.
    /// </summary>
    public static class LtiConstants
    {
        // ReSharper disable InconsistentNaming

        // Build a lookup table of role URNs
        static LtiConstants()
        {
            RoleUrns =  new Dictionary<string, Role>(StringComparer.OrdinalIgnoreCase);
            var type = typeof(Role);
            foreach (Role role in Enum.GetValues(type))
            {
                var memInfo = type.GetTypeInfo().GetMember(role.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(UrnAttribute), false).ToArray();
                var urn = ((UrnAttribute)attributes[0]).Urn;
                RoleUrns.Add(urn, role);
            }
        }

        /// <summary>
        /// Dictionary of Role URN's where the key is the short Role name (e.g. Learner).
        /// </summary>
        public static readonly Dictionary<string, Role> RoleUrns;

        #region LTI 1.0

        /// <summary>
        /// LTI Version 
        /// </summary>
        public const string LtiVersion = "LTI-1p0";

        /// <summary>
        /// Basic LTI launch message type
        /// </summary>
        public const string BasicLaunchLtiMessageType = "basic-lti-launch-request";

        /// <summary>
        /// Content-Item selection request message type
        /// </summary>
        public const string ContentItemSelectionRequestLtiMessageType = "ContentItemSelectionRequest";

        /// <summary>
        /// Content-Item selection response message type
        /// </summary>
        public const string ContentItemSelectionLtiMessageType = "ContentItemSelection";

        /// <summary>
        /// This is used internally to identify outcomes requests
        /// </summary>
        public const string OutcomesMessageType = "OutcomesRequest";

        #endregion

        #region Message Parameter Names

        /// <summary>
        /// accept_media_types parameter name.
        /// </summary>
        public const string AcceptMediaTypesParameter = "accept_media_types";

        /// <summary>
        /// accept_multiple parameter name.
        /// </summary>
        public const string AcceptMultipleParameter = "accept_multiple";

        /// <summary>
        /// accept_presentation_document_targets parameter name.
        /// </summary>
        public const string AcceptPresentationDocumentTargetsParameter = "accept_presentation_document_targets";

        /// <summary>
        /// accept_unsigned parameter name.
        /// </summary>
        public const string AcceptUnsignedParameter = "accept_unsigned";

        /// <summary>
        /// auto_create parameter name.
        /// </summary>
        public const string AutoCreateParameter = "auto_create";

        /// <summary>
        /// can_cofirm parameter name.
        /// </summary>
        public const string CanConfirmParameter = "can_confirm";

        /// <summary>
        /// confirm_url parameter name.
        /// </summary>
        public const string ConfirmUrlParameter = "confirm_url";

        /// <summary>
        /// data parameter name.
        /// </summary>
        public const string ContentItemDataParameter = "data";

        /// <summary>
        /// content_items parameter name.
        /// </summary>
        public const string ContentItemPlacementParameter = "content_items";

        /// <summary>
        /// content_item_return_url parameter name.
        /// </summary>
        public const string ContentItemReturnUrlParameter = "content_item_return_url";

        /// <summary>
        /// text parameter name.
        /// </summary>
        public const string ContentItemTextParameter = "text";

        /// <summary>
        /// title parameter name.
        /// </summary>
        public const string ContentItemTitleParameter = "title";

        /// <summary>
        /// context_id parameter name.
        /// </summary>
        public const string ContextIdParameter = "context_id";

        /// <summary>
        /// context_id_history parameter name.
        /// </summary>
        public const string ContextIdHistoryParameter = "context_id_history";

        /// <summary>
        /// context_label parameter name.
        /// </summary>
        public const string ContextLabelParameter = "context_label";

        /// <summary>
        /// context_title parameter name.
        /// </summary>
        public const string ContextTitleParameter = "context_title";

        /// <summary>
        /// context_type parameter name.
        /// </summary>
        public const string ContextTypeParameter = "context_type";

        /// <summary>
        /// launch_presentation_css_url parameter name.
        /// </summary>
        public const string LaunchPresentationCssUrlParameter = "launch_presentation_css_url";

        /// <summary>
        /// launch_presentation_document_target parameter name.
        /// </summary>
        public const string LaunchPresentationDocumentTargetParameter = "launch_presentation_document_target";

        /// <summary>
        /// launch_presentation_height parameter name.
        /// </summary>
        public const string LaunchPresentationHeightParameter = "launch_presentation_height";

        /// <summary>
        /// launch_presentation_locale parameter name.
        /// </summary>
        public const string LaunchPresentationLocaleParameter = "launch_presentation_locale";

        /// <summary>
        /// launch_presentation_return_url parameter name.
        /// </summary>
        public const string LaunchPresentationReturnUrlParameter = "launch_presentation_return_url";

        /// <summary>
        /// launch_presentation_width parameter name.
        /// </summary>
        public const string LaunchPresentationWidthParameter = "launch_presentation_width";

        /// <summary>
        /// lis_course_offering_sourcedid parameter name.
        /// </summary>
        public const string LisCourseOfferingSourcedIdParameter = "lis_course_offering_sourcedid";

        /// <summary>
        /// lis_course_section_sourcedid parameter name.
        /// </summary>
        public const string LisCourseSectionSourceIdParameter = "lis_course_section_sourcedid";

        /// <summary>
        /// lis_outcome_service_url parameter name.
        /// </summary>
        public const string LisOutcomeServiceUrlParameter = "lis_outcome_service_url";

        /// <summary>
        /// lis_person_contact_email_primary parameter name.
        /// </summary>
        public const string LisPersonContactEmailPrimaryParameter = "lis_person_contact_email_primary";

        /// <summary>
        /// lis_person_name_family parameter name.
        /// </summary>
        public const string LisPersonNameFamilyParameter = "lis_person_name_family";

        /// <summary>
        /// lis_person_name_full parameter name.
        /// </summary>
        public const string LisPersonNameFullParameter = "lis_person_name_full";

        /// <summary>
        /// lis_person_name_given parameter name.
        /// </summary>
        public const string LisPersonNameGivenParameter = "lis_person_name_given";

        /// <summary>
        /// lis_person_sourcedid parameter name.
        /// </summary>
        public const string LisPersonSourcedIdParameter = "lis_person_sourcedid";

        /// <summary>
        /// lis_result_sourcedid parameter name.
        /// </summary>
        public const string LisResultSourcedIdParameter = "lis_result_sourcedid";

        /// <summary>
        /// lti_errorlog parameter name.
        /// </summary>
        public const string LtiErrorLogParameter = "lti_errorlog";

        /// <summary>
        /// lti_errormsg parameter name.
        /// </summary>
        public const string LtiErrorMsgParameter = "lti_errormsg";

        /// <summary>
        /// lti_log parameter name.
        /// </summary>
        public const string LtiLogParameter = "lti_log";

        /// <summary>
        /// lti_msg parameter name.
        /// </summary>
        public const string LtiMsgParameter = "lti_msg";

        /// <summary>
        /// lti_message_type parameter name.
        /// </summary>
        public const string LtiMessageTypeParameter = "lti_message_type";

        /// <summary>
        /// lti_version parameter name.
        /// </summary>
        public const string LtiVersionParameter = "lti_version";

        /// <summary>
        /// resource_link_description parameter name.
        /// </summary>
        public const string ResourceLinkDescriptionParameter = "resource_link_description";

        /// <summary>
        /// resource_link_id parameter name.
        /// </summary>
        public const string ResourceLinkIdParameter = "resource_link_id";

        /// <summary>
        /// resource_link_id_history parameter name.
        /// </summary>
        public const string ResourceLinkIdHistoryParameter = "resource_link_id_history";

        /// <summary>
        /// resource_link_title parameter name.
        /// </summary>
        public const string ResourceLinkTitleParameter = "resource_link_title";

        /// <summary>
        /// role_score_mentor parameter name.
        /// </summary>
        public const string RoleScopeMentorParameter = "role_scope_mentor";

        /// <summary>
        /// roles parameter name.
        /// </summary>
        public const string RolesParameter = "roles";

        /// <summary>
        /// tool_consumer_info_product_family_code parameter name.
        /// </summary>
        public const string ToolConsumerInfoProductFamilyCodeParameter = "tool_consumer_info_product_family_code";

        /// <summary>
        /// tool_consumer_info_version parameter name.
        /// </summary>
        public const string ToolConsumerInfoVersionParameter = "tool_consumer_info_version";

        /// <summary>
        /// tool_consumer_instance_contact_email parameter name.
        /// </summary>
        public const string ToolConsumerInstanceContactEmailParameter = "tool_consumer_instance_contact_email";

        /// <summary>
        /// tool_consumer_instance_description parameter name.
        /// </summary>
        public const string ToolConsumerInstanceDescriptionParameter = "tool_consumer_instance_description";

        /// <summary>
        /// tool_consumer_instance_guid parameter name.
        /// </summary>
        public const string ToolConsumerInstanceGuidParameter = "tool_consumer_instance_guid";

        /// <summary>
        /// tool_consumer_instance_name parameter name.
        /// </summary>
        public const string ToolConsumerInstanceNameParameter = "tool_consumer_instance_name";

        /// <summary>
        /// tool_consumer_instance_url parameter name.
        /// </summary>
        public const string ToolConsumerInstanceUrlParameter = "tool_consumer_instance_url";

        /// <summary>
        /// user_id parameter name.
        /// </summary>
        public const string UserIdParameter = "user_id";

        /// <summary>
        /// user_image parameter name.
        /// </summary>
        public const string UserImageParameter = "user_image";

        #endregion

        #region Media types

        /// <summary>
        /// Outcomes 1.0 media type (application/xml).
        /// </summary>
        public const string ImsxOutcomeMediaType = "application/xml";

        /// <summary>
        /// application/vnd.ims.lis.v2.lineitemcontainer+json media type.
        /// </summary>
        public const string LineItemContainerMediaType = "application/vnd.ims.lis.v2.lineitemcontainer+json";

        /// <summary>
        /// application/vnd.ims.lis.v2.lineitem+json media type.
        /// </summary>
        public const string LineItemMediaType = "application/vnd.ims.lis.v2.lineitem+json";

        /// <summary>
        /// application/vnd.ims.lis.v2.lineitemresults+json media type.
        /// </summary>
        public const string LineItemResultsMediaType = "application/vnd.ims.lis.v2.lineitemresults+json";

        /// <summary>
        /// application/vnd.ims.lis.v2.resultcontainer+json media type.
        /// </summary>
        public const string LisResultContainerMediaType = "application/vnd.ims.lis.v2.resultcontainer+json";

        /// <summary>
        /// application/vnd.ims.lis.v2p1.result+json media type.
        /// </summary>
        public const string LisResultMediaType = "application/vnd.ims.lis.v2p1.result+json";

        /// <summary>
        /// application/vnd.ims.lti.v1.ltilink media type.
        /// </summary>
        public const string LtiLinkMediaType = "application/vnd.ims.lti.v1.ltilink";

        /// <summary>
        /// application/vnd.ims.lti.v1.outcome+xml media type.
        /// </summary>
        public const string OutcomeMediaType = "application/vnd.ims.lti.v1.outcome+xml";

        /// <summary>
        /// application/vnd.ims.lti.v2.toolconsumerprofile+json media type.
        /// </summary>
        public const string ToolConsumerProfileMediaType = "application/vnd.ims.lti.v2.toolconsumerprofile+json";

        #endregion

        #region Object types

        /// <summary>
        /// Activity object type.
        /// </summary>
        public const string ActivityType = "Activity";

        /// <summary>
        /// ContentItem object type.
        /// </summary>
        public const string ContentItemType = "ContentItem";

        /// <summary>
        /// Context object type.
        /// </summary>
        public const string ContextType = "Context";

        /// <summary>
        /// FileItem object type.
        /// </summary>
        public const string FileItemType = "FileItem";

        /// <summary>
        /// Page object type.
        /// </summary>
        public const string LineItemContainerPageType = "Page";

        /// <summary>
        /// LineItemContainer object type.
        /// </summary>
        public const string LineItemContainerType = "LineItemContainer";

        /// <summary>
        /// LineItem object type.
        /// </summary>
        public const string LineItemType = "LineItem";

        /// <summary>
        /// LISResult object type.
        /// </summary>
        public const string LisResultType = "LISResult";

        /// <summary>
        /// LisResultContainer object type.
        /// </summary>
        public const string LisResultContainerType = "LisResultContainer";

        /// <summary>
        /// LtiLinkItem object type.
        /// </summary>
        public const string LtiLinkType = "LtiLinkItem";

        /// <summary>
        /// NumericLimits object type.
        /// </summary>
        public const string NumericLimitsType = "NumericLimits";

        /// <summary>
        /// RestService object type.
        /// </summary>
        public const string RestService = "RestService";

        /// <summary>
        /// ToolConsumerProfile object type.
        /// </summary>
        public const string ToolConsumerProfileType = "ToolConsumerProfile";

        #endregion

        #region JSON-LD context Ids

        /// <summary>
        /// http://purl.imsglobal.org/ctx/lti/v1/ContentItem context id.
        /// </summary>
        public static readonly Uri ContentItemContextId = new Uri("http://purl.imsglobal.org/ctx/lti/v1/ContentItem");

        /// <summary>
        /// http://purl.imsglobal.org/ctx/lis/v2/outcomes/LineItemContainer context id.
        /// </summary>
        public static readonly Uri LineItemContainerContextId = new Uri("http://purl.imsglobal.org/ctx/lis/v2/outcomes/LineItemContainer");

        /// <summary>
        /// http://purl.imsglobal.org/ctx/lis/v2/LineItem context id.
        /// </summary>
        public static readonly Uri LineItemContextId = new Uri("http://purl.imsglobal.org/ctx/lis/v2/LineItem");

        /// <summary>
        /// http://purl.imsglobal.org/vocab/lis/v2/outcomes# context id.
        /// </summary>
        public static readonly Uri OutcomesVocabularyContextId = new Uri("http://purl.imsglobal.org/vocab/lis/v2/outcomes#");

        /// <summary>
        /// http://purl.imsglobal.org/ctx/lis/v2/outcomes/ResultContainer context id.
        /// </summary>
        public static readonly Uri ResultContainerContextId = new Uri("http://purl.imsglobal.org/ctx/lis/v2/outcomes/ResultContainer");

        /// <summary>
        /// http://purl.imsglobal.org/ctx/lis/v2p1/Result context id.
        /// </summary>
        public static readonly Uri ResultContextId = new Uri("http://purl.imsglobal.org/ctx/lis/v2p1/Result");

        /// <summary>
        /// http://purl.imsglobal.org/ctx/lti/v2/ToolConsumerProfile context id.
        /// </summary>
        public static readonly Uri ToolConsumerProfileContextId = new Uri("http://purl.imsglobal.org/ctx/lti/v2/ToolConsumerProfile");

        #endregion

        #region Encodings

        /// <summary>
        /// en encoding.
        /// </summary>
        public const string ScoreLanguage = "en";

        #endregion

        // ReSharper restore InconsistentNaming
    }
}
