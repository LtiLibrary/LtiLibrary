using LtiLibrary.Common;
using LtiLibrary.ContentItems;
using LtiLibrary.OAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using LtiLibrary.Outcomes;

namespace LtiLibrary.Lti1
{
    public class LtiRequest 
        : OAuthRequest, IBasicLaunchRequest, IOutcomesManagementRequest, IContentItemSelectionRequest, IContentItemSelectionResponse
    {
        public LtiRequest() : this(null) {}
        public LtiRequest(string messageType)
        {
            // Create empty request if no messageType
            if (string.IsNullOrEmpty(messageType)) return;

            // OAuth defaults
            CallBack = OAuthConstants.CallbackDefault;
            Nonce = Guid.NewGuid().ToString("N");
            SignatureMethod = OAuthConstants.SignatureMethodHmacSha1;
            TimestampAsDateTime = DateTime.UtcNow;
            Version = OAuthConstants.Version10;

            // LTI defaults
            LaunchPresentationLocale = CultureInfo.CurrentCulture.Name;
            LtiMessageType = messageType;
            LtiVersion = LtiConstants.LtiVersion;
        }

        #region ILtiRequest Parameters

        /// <summary>
        /// This is an opaque identifier that uniquely identifies the context that contains the link being launched.
        /// <para>
        /// Parameter: context_id.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ContextIdParameter)]
        public string ContextId
        {
            get
            {
                return Parameters[LtiConstants.ContextIdParameter];
            }
            set
            {
                Parameters[LtiConstants.ContextIdParameter] = value;
            }
        }

        /// <summary>
        /// Comma-separated lists of IDs under which the context has been previously known (i.e. from which it has been copied).
        /// Each ID should be URL-encoded in case it contains a comma. The IDs should listed in reverse chronological order 
        /// (i.e. latest first). Only the most recent ID need be implemented to support these variables.
        /// <para>
        /// Parameter: context_id.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ContextIdHistoryParameter)]
        public string ContextIdHistory
        {
            get
            {
                return Parameters[LtiConstants.ContextIdHistoryParameter];
            }
            set
            {
                Parameters[LtiConstants.ContextIdHistoryParameter] = value;
            }
        }

        /// <summary>
        /// A plain text label for the context – intended to fit in a column.
        /// <para>
        /// Parameter: context_label.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ContextLabelParameter)]
        public string ContextLabel
        {
            get
            {
                return Parameters[LtiConstants.ContextLabelParameter];
            }
            set
            {
                Parameters[LtiConstants.ContextLabelParameter] = value;
            }
        }

        /// <summary>
        /// A plain text title of the context – it should be about the length of a line.
        /// <para>
        /// Parameter: context_title.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ContextTitleParameter)]
        public string ContextTitle
        {
            get
            {
                return Parameters[LtiConstants.ContextTitleParameter];
            }
            set
            {
                Parameters[LtiConstants.ContextTitleParameter] = value;
            }
        }

        /// <summary>
        /// This string is a comma-separated list of URN values that identify the type of context. 
        /// At a minimum, the list MUST include a URN value drawn from the LIS vocabulary. 
        /// The assumed namespace of these URNs is the LIS vocabulary so TCs can use the handles when the intent 
        /// is to refer to an LIS context type. 
        /// If the TC wants to include a context type from another namespace, a fully-qualified URN should be used.
        /// <para>
        /// Parameter: context_type.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [NotMapped]
        [DataMember(Name = LtiConstants.ContextTypeParameter)]
        public LisContextType? ContextType
        {
            get
            {
                LisContextType contextType;
                return Enum.TryParse(Parameters[LtiConstants.ContextTypeParameter], out contextType)
                   ? contextType
                   : default(LisContextType?);
            }
            set
            {
                Parameters[LtiConstants.ContextTypeParameter] = Convert.ToString(value);
            }
        }

        /// <summary>
        /// This is a URL to an LMS-specific CSS URL. There are no standards that describe exactly what CSS classes, etc. 
        /// should be in this CSS. The TC could send its standard CSS URL that it would apply to its local tools. 
        /// The TC should include styling for HTML tags to set font, color, etc. and also include its proprietary tags 
        /// used to style its internal tools.
        /// <para>
        /// Parameter: launch_presentation_css.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LaunchPresentationCssUrlParameter)]
        public string LaunchPresentationCssUrl
        {
            get
            {
                return Parameters[LtiConstants.LaunchPresentationCssUrlParameter];
            }
            set
            {
                Parameters[LtiConstants.LaunchPresentationCssUrlParameter] = value;
            }
        }

        /// <summary>
        /// The value should be either ‘frame’, ‘iframe’ or ‘window’. 
        /// This field communicates the kind of browser window/frame where the TC has launched the tool. 
        /// The TP can ignore this parameter and detect its environment through JavaScript, but this parameter gives 
        /// the TP the information without requiring the use of JavaScript if the tool prefers.
        /// <para>
        /// Parameter: launch_presentation_document_target.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LaunchPresentationDocumentTargetParameter)]
        public DocumentTarget? LaunchPresentationDocumentTarget
        {
            get
            {
                DocumentTarget presentationTarget;
                return Enum.TryParse(Parameters[LtiConstants.LaunchPresentationDocumentTargetParameter], out presentationTarget)
                   ? presentationTarget
                   : default(DocumentTarget?);
            }
            set
            {
                Parameters[LtiConstants.LaunchPresentationDocumentTargetParameter] = Convert.ToString(value);
            }
        }

        /// <summary>
        /// The height of the window or frame where the content from the tool will be displayed. 
        /// The tool can ignore this parameter and detect its environment through JavaScript, but this parameter gives 
        /// the TP the information without requiring the use of JavaScript if the tool prefers.
        /// <para>
        /// Parameter: launch_presentation_height.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LaunchPresentationHeightParameter)]
        public int? LaunchPresentationHeight
        {
            get
            {
                int value;
                return int.TryParse(Parameters[LtiConstants.LaunchPresentationHeightParameter], out value)
                    ? value
                    : default(int?);
            }
            set
            {
                Parameters[LtiConstants.LaunchPresentationHeightParameter] = Convert.ToString(value);
            }
        }

        /// <summary>
        /// Language, country and variant as represented using the IETF Best Practices for Tags for 
        /// Identifying Languages (BCP-47) available at http://www.rfc-editor.org/rfc/bcp/bcp47.txt.
        /// <para>
        /// Parameter: launch_presentation_locale.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LaunchPresentationLocaleParameter)]
        public string LaunchPresentationLocale
        {
            get
            {
                return Parameters[LtiConstants.LaunchPresentationLocaleParameter];
            }
            set
            {
                Parameters[LtiConstants.LaunchPresentationLocaleParameter] = value;
            }
        }

        /// <summary>
        /// Fully qualified URL where the TP can redirect the user back to the TC interface. 
        /// This URL can be used once the TP is finished or if the TP cannot start or has some technical difficulty. 
        /// In the case of an error, the TP may add a parameter called lti_errormsg that includes some detail as to 
        /// the nature of the error. The lti_errormsg value should make sense if displayed to the user. 
        /// If the tool has displayed a message to the end user and only wants to give the TC a message to log, 
        /// use the parameter lti_errorlog instead of lti_errormsg. 
        /// If the tool is terminating normally, and wants a message displayed to the user it can include a text 
        /// message as the lti_msg parameter to the return URL. 
        /// If the tool is terminating normally and wants to give the TC a message to log, use the parameter lti_log. 
        /// This data should be sent on the URL as a GET – so the TP should take care to keep the overall length 
        /// of the parameters small enough to fit within the limitations of a GET request.
        /// <para>
        /// Parameter: launch_presentation_return_url.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LaunchPresentationReturnUrlParameter)]
        public string LaunchPresentationReturnUrl
        {
            get
            {
                return Parameters[LtiConstants.LaunchPresentationReturnUrlParameter];
            }
            set
            {
                Parameters[LtiConstants.LaunchPresentationReturnUrlParameter] = value;
            }
        }

        /// <summary>
        /// The width of the window or frame where the content from the tool will be displayed. 
        /// The tool can ignore this parameter and detect its environment through JavaScript, but this parameter gives 
        /// the TP the information without requiring the use of JavaScript if the tool prefers.
        /// <para>
        /// Parameter: launch_presentation_width.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LaunchPresentationHeightParameter)]
        public int? LaunchPresentationWidth
        {
            get
            {
                int value;
                return int.TryParse(Parameters[LtiConstants.LaunchPresentationHeightParameter], out value)
                    ? value
                    : default(int?);
            }
            set
            {
                Parameters[LtiConstants.LaunchPresentationHeightParameter] = Convert.ToString(value);
            }
        }

        /// <summary>
        /// This field contains a LIS course identifier associated with the context of this launch. 
        /// <para>
        /// Parameter: lis_course_offering_sourcedid.
        /// Custom parameter substitution: $CourseOffering.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LisCourseOfferingSourcedIdParameter)]
        public string LisCourseOfferingSourcedId
        {
            get
            {
                return Parameters[LtiConstants.LisCourseOfferingSourcedIdParameter];
            }
            set
            {
                Parameters[LtiConstants.LisCourseOfferingSourcedIdParameter] = value;
            }
        }

        /// <summary>
        /// This field contains a LIS course identifier associated with the context of this launch.
        /// <para>
        /// Parameter: lis_course_section_sourcedid.
        /// Custom parameter substitution: $CourseSection.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LisCourseSectionSourceIdParameter)]
        public string LisCourseSectionSourcedId
        {
            get
            {
                return Parameters[LtiConstants.LisCourseSectionSourceIdParameter];
            }
            set
            {
                Parameters[LtiConstants.LisCourseSectionSourceIdParameter] = value;
            }
        }

        /// <summary>
        /// This field contains the primary email address of the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_contact_email_primary.
        /// Custom parameter substitution: $Person.email.primary.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended unless it is suppressed because of privacy settings.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LisPersonContactEmailPrimaryParameter)]
        public string LisPersonEmailPrimary
        {
            get
            {
                return Parameters[LtiConstants.LisPersonContactEmailPrimaryParameter];
            }
            set
            {
                Parameters[LtiConstants.LisPersonContactEmailPrimaryParameter] = value;
            }
        }

        /// <summary>
        /// This field contains the family name of the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_name_family.
        /// Custom parameter substitution: $Person.name.family.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended unless it is suppressed because of privacy settings.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LisPersonNameFamilyParameter)]
        public string LisPersonNameFamily
        {
            get
            {
                return Parameters[LtiConstants.LisPersonNameFamilyParameter];
            }
            set
            {
                Parameters[LtiConstants.LisPersonNameFamilyParameter] = value;
            }
        }

        /// <summary>
        /// This field contains the full name of the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_name_full.
        /// Custom parameter substitution: $Person.name.full.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended unless it is suppressed because of privacy settings.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LisPersonNameFullParameter)]
        public string LisPersonNameFull
        {
            get
            {
                return Parameters[LtiConstants.LisPersonNameFullParameter];
            }
            set
            {
                Parameters[LtiConstants.LisPersonNameFullParameter] = value;
            }
        }

        /// <summary>
        /// This field contains the given name of the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_name_given.
        /// Custom parameter substitution: $Person.name.given.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended unless it is suppressed because of privacy settings.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LisPersonNameGivenParameter)]
        public string LisPersonNameGiven
        {
            get
            {
                return Parameters[LtiConstants.LisPersonNameGivenParameter];
            }
            set
            {
                Parameters[LtiConstants.LisPersonNameGivenParameter] = value;
            }
        }

        /// <summary>
        /// This field contains the LIS identifier for the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_sourcedid.
        /// Custom parameter substitution: $Person.sourcedId.
        /// Versions: 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LisPersonSourcedIdParameter)]
        public string LisPersonSourcedId
        {
            get
            {
                return Parameters[LtiConstants.LisPersonSourcedIdParameter];
            }
            set
            {
                Parameters[LtiConstants.LisPersonSourcedIdParameter] = value;
            }
        }

        /// <summary>
        /// The type of message.
        /// <para>
        /// Parameter: lti_message_type.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Required.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LtiMessageTypeParameter)]
        public string LtiMessageType
        {
            get
            {
                return Parameters[LtiConstants.LtiMessageTypeParameter];
            }
            set
            {
                Parameters[LtiConstants.LtiMessageTypeParameter] = value;
            }
        }

        /// <summary>
        /// This indicates which version of the specification is being used for this particular message. 
        /// Since launches for version 1.1 are upwards compatible with 1.0 launches, this value is not advanced for LTI 1.1.
        /// <para>
        /// Parameter: lti_version.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Required.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LtiVersionParameter)]
        public string LtiVersion
        {
            get
            {
                return Parameters[LtiConstants.LtiVersionParameter];
            }
            set
            {
                Parameters[LtiConstants.LtiVersionParameter] = value;
            }
        }

        /// <summary>
        /// A list of URN values for roles. If this list is non-empty, it should contain at least one role from the LIS System Role, 
        /// LIS Institution Role, or LIS Context Role vocabularies. 
        /// The assumed namespace of these URNs is the LIS vocabulary of LIS Context Roles so TCs can use the handles when the intent 
        /// is to refer to an LIS context role. 
        /// If the TC wants to include a role from another namespace, a fully-qualified URN should be used. Usage of roles from non-LIS 
        /// vocabularies is discouraged as it may limit interoperability.
        /// <para>
        /// Parameter: roles.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.RolesParameter)]
        public string Roles
        {
            get
            {
                return Parameters[LtiConstants.RolesParameter];
            }
            set
            {
                Parameters[LtiConstants.RolesParameter] = value;
            }
        }

        /// <summary>
        /// In order to better assist tools in using extensions and also making their user interface fit into the TC's 
        /// user interface that they are being called from, each TC is encouraged to include the this parameter.
        /// <para>
        /// Parameter: tool_consumer_info_product_family_code.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ToolConsumerInfoProductFamilyCodeParameter)]
        public string ToolConsumerInfoProductFamilyCode
        {
            get
            {
                return Parameters[LtiConstants.ToolConsumerInfoProductFamilyCodeParameter];
            }
            set
            {
                Parameters[LtiConstants.ToolConsumerInfoProductFamilyCodeParameter] = value;
            }
        }

        /// <summary>
        /// This field should have a major release number followed by a period. The format of the minor release is flexible. 
        /// The TP should be flexible when parsing this field.
        /// <para>
        /// Parameter: tool_consumer_info_version.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ToolConsumerInfoVersionParameter)]
        public string ToolConsumerInfoVersion
        {
            get
            {
                return Parameters[LtiConstants.ToolConsumerInfoVersionParameter];
            }
            set
            {
                Parameters[LtiConstants.ToolConsumerInfoVersionParameter] = value;
            }
        }

        /// <summary>
        /// An email contact for the TC instance.
        /// <para>
        /// Parameter: tool_consumer_instance_contact_email.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ToolConsumerInstanceContactEmailParameter)]
        public string ToolConsumerInstanceContactEmail
        {
            get
            {
                return Parameters[LtiConstants.ToolConsumerInstanceContactEmailParameter];
            }
            set
            {
                Parameters[LtiConstants.ToolConsumerInstanceContactEmailParameter] = value;
            }
        }

        /// <summary>
        /// This is a plain text user visible field – it should be about the length of a line.
        /// <para>
        /// Parameter: tool_consumer_instance_description.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ToolConsumerInstanceDescriptionParameter)]
        public string ToolConsumerInstanceDescription
        {
            get
            {
                return Parameters[LtiConstants.ToolConsumerInstanceDescriptionParameter];
            }
            set
            {
                Parameters[LtiConstants.ToolConsumerInstanceDescriptionParameter] = value;
            }
        }

        /// <summary>
        /// This is a unique identifier for the TC. 
        /// A common practice is to use the DNS of the organization or the DNS of the TC instance. 
        /// If the organization has multiple TC instances, then the best practice is to prefix the domain name with a 
        /// locally unique identifier for the TC instance. 
        /// In the single-tenancy case, the tool consumer data can be often be derived from the oauth_consumer_key. 
        /// In a multi-tenancy case this can be used to differentiate between the multiple tenants within a single 
        /// installation of a Tool Consumer.
        /// <para>
        /// Parameter: tool_consumer_instance_guid.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended in systems capable of multi-tenancy.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ToolConsumerInstanceGuidParameter)]
        public string ToolConsumerInstanceGuid
        {
            get
            {
                return Parameters[LtiConstants.ToolConsumerInstanceGuidParameter];
            }
            set
            {
                Parameters[LtiConstants.ToolConsumerInstanceGuidParameter] = value;
            }
        }

        /// <summary>
        /// This is a plain text user visible field – it should be about the length of a column.
        /// <para>
        /// Parameter: tool_consumer_instance_name.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ToolConsumerInstanceNameParameter)]
        public string ToolConsumerInstanceName
        {
            get
            {
                return Parameters[LtiConstants.ToolConsumerInstanceNameParameter];
            }
            set
            {
                Parameters[LtiConstants.ToolConsumerInstanceNameParameter] = value;
            }
        }

        /// <summary>
        /// This is the URL of the consumer instance. This parameter is optional.
        /// <para>
        /// Parameter: tool_consumer_instance_url.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ToolConsumerInstanceUrlParameter)]
        public string ToolConsumerInstanceUrl
        {
            get
            {
                return Parameters[LtiConstants.ToolConsumerInstanceUrlParameter];
            }
            set
            {
                Parameters[LtiConstants.ToolConsumerInstanceUrlParameter] = value;
            }
        }

        /// <summary>
        /// Uniquely identifies the user. 
        /// This should not contain any identifying information for the user. 
        /// Best practice is that this field should be a TC-generated long-term “primary key” to the user record – not the “logical key". 
        /// At a minimum, this value needs to be unique within a TC. 
        /// <para>
        /// Parameter: user_id.
        /// Custom parameter substitution: $User.id.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.UserIdParameter)]
        public string UserId
        {
            get
            {
                return Parameters[LtiConstants.UserIdParameter];
            }
            set
            {
                Parameters[LtiConstants.UserIdParameter] = value;
            }
        }

        /// <summary>
        /// This attribute specifies the URI for an image of the user who launches this request. 
        /// This image is suitable for use as a "profile picture" or an avatar representing the user. 
        /// It is expected to be a relatively small graphic image file using a widely supported image format 
        /// (i.e., PNG, JPG, or GIF) with a square aspect ratio.
        /// <para>
        /// Parameter: user_image.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.UserImageParameter)]
        public string UserImage
        {
            get
            {
                return Parameters[LtiConstants.UserImageParameter];
            }
            set
            {
                Parameters[LtiConstants.UserImageParameter] = value;
            }
        }

        #endregion

        #region IBasicLaunchRequest Parameters

        /// <summary>
        /// A plain text description of the link’s destination, suitable for display alongside the link. 
        /// Typically no more than a few lines long.
        /// <para>
        /// Parameter: resource_link_description.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ResourceLinkDescriptionParameter)]
        public string ResourceLinkDescription
        {
            get
            {
                return Parameters[LtiConstants.ResourceLinkDescriptionParameter];
            }
            set
            {
                Parameters[LtiConstants.ResourceLinkDescriptionParameter] = value;
            }
        }

        /// <summary>
        /// This is an opaque unique identifier that the TC guarantees will be unique within the TC for every placement of the link. 
        /// If the tool/activity is placed multiple times in the same context, each of those placements will be distinct. 
        /// This value will also change if the item is exported from one system or context and imported into another system or context.
        /// <para>
        /// Parameter: resource_link_id.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Required.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ResourceLinkIdParameter)]
        public string ResourceLinkId
        {
            get
            {
                return Parameters[LtiConstants.ResourceLinkIdParameter];
            }
            set
            {
                Parameters[LtiConstants.ResourceLinkIdParameter] = value;
            }
        }

        /// <summary>
        /// Comma-separated lists of IDs under which the resource link has been previously known (i.e. from which it has been copied).
        /// Each ID should be URL-encoded in case it contains a comma. The IDs should listed in reverse chronological order 
        /// (i.e. latest first). Only the most recent ID need be implemented to support these variables.
        /// <para>
        /// Parameter: context_id.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ResourceLinkIdHistoryParameter)]
        public string ResourceLinkIdHistory
        {
            get
            {
                return Parameters[LtiConstants.ResourceLinkIdHistoryParameter];
            }
            set
            {
                Parameters[LtiConstants.ResourceLinkIdHistoryParameter] = value;
            }
        }

        /// <summary>
        /// A plain text title for the resource. This is the clickable text that appears in the link.
        /// <para>
        /// Parameter: resource_link_title.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.ResourceLinkTitleParameter)]
        public string ResourceLinkTitle
        {
            get
            {
                return Parameters[LtiConstants.ResourceLinkTitleParameter];
            }
            set
            {
                Parameters[LtiConstants.ResourceLinkTitleParameter] = value;
            }
        }

        /// <summary>
        /// A comma separated list of the user_id values which the current user can access as a mentor. 
        /// The typical use case for this parameter is where the Mentor role represents a parent, guardian or auditor. 
        /// It may be used in different ways by each TP, but the general expectation is that the mentor will be provided with
        /// access to tracking and summary information, but not necessarily the user’s personal content or assignment submissions. 
        /// In order to accommodate user_id values which contain a comma, each user_id should be url-encoded. 
        /// This also means that each user_id from the comma separated list should url-decoded before a TP uses it. 
        /// This parameter should only be used when one of the roles passed for the current user is for urn:lti:role:ims/lis/Mentor.
        /// <para>
        /// Parameter: role_scope_mentor.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.RoleScopeMentorParameter)]
        public string RoleScopeMentor
        {
            get
            {
                return Parameters[LtiConstants.RoleScopeMentorParameter];
            }
            set
            {
                Parameters[LtiConstants.RoleScopeMentorParameter] = value;
            }
        }

        #endregion

        #region IOutcomesManagementRequest Parameters


        /// <summary>
        /// This field should be no more than 1023 characters long. 
        /// This value should not change from one launch to the next and in general, the TP can expect that 
        /// there is a one-to-one mapping between the lis_outcome_service_url and a particular oauth_consumer_key. 
        /// This value might change if there was a significant re-configuration of the TC system or if the TC 
        /// moved from one domain to another. 
        /// The TP can assume that this URL generally does not change from one launch to the next but should be 
        /// able to deal with cases where this value rarely changes. 
        /// The service URL may support various operations/actions. 
        /// The Basic Outcomes Service Provider will respond with a response of 'unimplemented' for actions it 
        /// does not support. 
        /// <para>
        /// Parameter: lis_outcome_service_url.
        /// Custom parameter substitution: none.
        /// Versions: 1.1, 1.2.
        /// Required if the TC is accepting outcomes for any launches associated with the resource_link_id.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LisOutcomeServiceUrlParameter)]
        public string LisOutcomeServiceUrl
        {
            get
            {
                return Parameters[LtiConstants.LisOutcomeServiceUrlParameter];
            }
            set
            {
                Parameters[LtiConstants.LisOutcomeServiceUrlParameter] = value;
            }
        }

        /// <summary>
        /// This field contains an identifier that indicates the LIS Result Identifier (if any) associated with this launch. 
        /// This field identifies a unique row and column within the TC gradebook. 
        /// This field is unique for every combination of context_id/resource_link_id/user_id. 
        /// This value may change for a particular resource_link_id/user_id from one launch to the next. 
        /// The TP should only retain the most recent value for this field for a particular resource_link_id/user_id.
        /// <para>
        /// Parameter: lis_result_sourcedid.
        /// Custom parameter substitution: $Result.sourcedGUID.
        /// Versions: 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [DataMember(Name = LtiConstants.LisResultSourcedIdParameter)]
        public string LisResultSourcedId
        {
            get
            {
                return Parameters[LtiConstants.LisResultSourcedIdParameter];
            }
            set
            {
                Parameters[LtiConstants.LisResultSourcedIdParameter] = value;
            }
        }

        #endregion

        #region IContentItemRequest Parameters

        /// <summary>
        /// This indicates whether the TC is able and willing to make a local copy of a content item. The return 
        /// message may include a expiresAt parameter to indicate that the URL provided will expire and so a copy 
        /// of the content item should be stored locally before the expiry time passes. Use a value of false (the 
        /// default) to indicate that the TC has no capacity for storing local copies of content items.
        /// </summary>
        [DataMember(Name = LtiConstants.AcceptCopyAdviceParameter)]
        public bool? AcceptCopyAdvice
        {
            get
            {
                if (string.IsNullOrEmpty(Parameters[LtiConstants.AcceptCopyAdviceParameter]))
                {
                    return null;
                }

                bool value;
                return Boolean.TryParse(Parameters[LtiConstants.AcceptCopyAdviceParameter], out value) && value;
            }
            set
            {
                Parameters[LtiConstants.AcceptCopyAdviceParameter] = value.HasValue ? value.ToString().ToLowerInvariant() : null;
            }
        }

        /// <summary>
        /// A comma-separated list of MIME types which can be accepted on the return URL. A MIME type of 
        /// application/vnd.ims.lti.v1.launch+json is used to represent an LTI launch request to a TP. This 
        /// parameter should use the same format as the Accept header in HTTP requests [HTTP, 99]. For example, 
        /// a value of “image/*; q=0.5, image/png” indicates that a PNG image is preferred, but any type of 
        /// image will be accepted if one is not available.
        /// </summary>
        [DataMember(Name = LtiConstants.AcceptMediaTypesParameter)]
        public string AcceptMediaTypes
        {
            get
            {
                return Parameters[LtiConstants.AcceptMediaTypesParameter];
            }
            set
            {
                Parameters[LtiConstants.AcceptMediaTypesParameter] = value;
            }
        }

        /// <summary>
        /// This indicates whether the user should be permitted to select more than one item. This parameter is 
        /// optional; when omitted a value of false should be assumed (i.e. only a single item may be returned).
        /// </summary>
        [DataMember(Name = LtiConstants.AcceptMultipleParameter)]
        public bool? AcceptMultiple
        {
            get
            {
                if (string.IsNullOrEmpty(Parameters[LtiConstants.AcceptMultipleParameter]))
                {
                    return null;
                }
                
                bool value;
                return Boolean.TryParse(Parameters[LtiConstants.AcceptMultipleParameter], out value) && value;
            }
            set
            {
                Parameters[LtiConstants.AcceptMultipleParameter] = value.HasValue ? value.ToString().ToLowerInvariant() : null;
            }
        }

        /// <summary>
        /// A comma-separated list of ways in which the selected content item(s) can be requested to be opened 
        /// (via the presentation_document_target response parameter).
        /// </summary>
        [DataMember(Name = LtiConstants.AcceptPresentationDocumentTargetsParameter)]
        public string AcceptPresentationDocumentTargets
        {
            get
            {
                return Parameters[LtiConstants.AcceptPresentationDocumentTargetsParameter];
            }
            set
            {
                Parameters[LtiConstants.AcceptPresentationDocumentTargetsParameter] = value;
            }
        }

        /// <summary>
        /// This indicates whether the TC is willing to accept an unsigned return message, or not. A signed message 
        /// should always be required when the content item is being created automatically in the Tool Consumer 
        /// without further interaction from the user. This parameter is optional; when omitted a value of false 
        /// should be assumed (i.e. the return message should be signed).
        /// </summary>
        [DataMember(Name = LtiConstants.AcceptUnsignedParameter)]
        public bool? AcceptUnsigned
        {
            get
            {
                if (string.IsNullOrEmpty(Parameters[LtiConstants.AcceptUnsignedParameter]))
                {
                    return null;
                }

                bool value;
                return Boolean.TryParse(Parameters[LtiConstants.AcceptUnsignedParameter], out value) && value;
            }
            set
            {
                Parameters[LtiConstants.AcceptUnsignedParameter] = value.HasValue ? value.ToString().ToLowerInvariant() : null;
            }
        }

        /// <summary>
        /// Fully qualified URL where the TP redirects the user back to the TC interface. This URL can be used 
        /// once the TP is finished or if the TP cannot start or has some technical difficulty.
        /// </summary>
        [DataMember(Name = LtiConstants.ContentItemReturnUrlParameter)]
        public string ContentItemReturnUrl
        {
            get
            {
                return Parameters[LtiConstants.ContentItemReturnUrlParameter];
            }
            set
            {
                Parameters[LtiConstants.ContentItemReturnUrlParameter] = value;
            }
        }

        /// <summary>
        /// Fully qualified URL of the service that accepts Content-Items. If it is not sent to the TP,
        /// the TP will return to ContentItemReturnUrl with the Content-Items.
        /// </summary>
        [DataMember(Name = LtiConstants.ContentItemServiceUrlParameter)]
        public string ContentItemServiceUrl
        {
            get
            {
                return Parameters[LtiConstants.ContentItemServiceUrlParameter];
            }
            set
            {
                Parameters[LtiConstants.ContentItemServiceUrlParameter] = value;
            }
        }

        /// <summary>
        /// An opaque value which should be returned by the TP in its response.
        /// </summary>
        [DataMember(Name = LtiConstants.ContentItemDataParameter)]
        public string Data
        {
            get
            {
                return Parameters[LtiConstants.ContentItemDataParameter];
            }
            set
            {
                Parameters[LtiConstants.ContentItemDataParameter] = value;
            }
        }

        /// <summary>
        /// Default text to be used as the visible text or titles for the content-item returned by the TP.
        /// </summary>
        [DataMember(Name = LtiConstants.ContentItemTextParameter)]
        public string Text
        {
            get
            {
                return Parameters[LtiConstants.ContentItemTextParameter];
            }
            set
            {
                Parameters[LtiConstants.ContentItemTextParameter] = value;
            }
        }

        /// <summary>
        /// Default text to be used as the title or alt text for the content-item returned by the TP. If no title 
        /// is returned by the TP, the TC may use the text parameter (if any) instead.
        /// </summary>
        [DataMember(Name = LtiConstants.ContentItemTitleParameter)]
        public string Title
        {
            get
            {
                return Parameters[LtiConstants.ContentItemTitleParameter];
            }
            set
            {
                Parameters[LtiConstants.ContentItemTitleParameter] = value;
            }
        }

        #endregion

        #region IContentItemResponse Parameters

        /// <summary>
        /// The value of this parameter should be a JSON array containing details of each of the items selected (see examples below). If no items have been selected this parameter may contain an empty array or be omitted.
        /// </summary>
        [DataMember(Name = LtiConstants.ContentItemPlacementParameter)]
        public string ContentItems
        {
            get
            {
                return Parameters[LtiConstants.ContentItemPlacementParameter];
            }
            set
            {
                Parameters[LtiConstants.ContentItemPlacementParameter] = value;
            }
        }

        /// <summary>
        /// This parameter may contain a plain text error message to be logged by the TC.
        /// </summary>
        [DataMember(Name = LtiConstants.LtiErrorLogParameter)]
        public string LtiErrorLog
        {
            get
            {
                return Parameters[LtiConstants.LtiErrorLogParameter];
            }
            set
            {
                Parameters[LtiConstants.LtiErrorLogParameter] = value;
            }
        }

        /// <summary>
        /// In the case of an error, the TP may use this parameter to provide some detail in plain text as to the nature of the error for displaying to the user. The TC should ensure that this message is displayed; if the TP has already provided an indication of the error to the user, then there would be no need to also use this parameter.
        /// </summary>
        [DataMember(Name = LtiConstants.LtiErrorMsgParameter)]
        public string LtiErrorMsg
        {
            get
            {
                return Parameters[LtiConstants.LtiErrorMsgParameter];
            }
            set
            {
                Parameters[LtiConstants.LtiErrorMsgParameter] = value;
            }
        }

        /// <summary>
        /// This parameter allows the TP to give the TC a plain text message to log when it returns normally.
        /// </summary>
        [DataMember(Name = LtiConstants.LtiLogParameter)]
        public string LtiLog
        {
            get
            {
                return Parameters[LtiConstants.LtiLogParameter];
            }
            set
            {
                Parameters[LtiConstants.LtiLogParameter] = value;
            }
        }

        /// <summary>
        /// If the TP is returning normally, and wants a message displayed to the user it can include it as a plain text1 value in this parameter.
        /// </summary>
        [DataMember(Name = LtiConstants.LtiMsgParameter)]
        public string LtiMsg
        {
            get
            {
                return Parameters[LtiConstants.LtiMsgParameter];
            }
            set
            {
                Parameters[LtiConstants.LtiMsgParameter] = value;
            }
        }

        #endregion

        // These properties are used during custom variable substition. They are not normally
        // part of an LTI request.
        #region Custom Substitution Values

        /// <summary>
        /// A URI describing the context's organisational properties; for example, an ldap:// URI. 
        /// Multiple URIs can be separated using commas.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Context.org.
        /// Versions: 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string ContextOrg { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.academicSession.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseOfferingAcademicSession { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseOfferingCourseNumber { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseOfferingCredits { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseOfferingLabel { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseOfferingLongDescription { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseOfferingShortDescription { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseOfferingTitle { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionCourseNumber { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionCredits { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionDataSource { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.dept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionDept { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.enrollControl.accept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionEnrollControlAccept { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.enrollControl.allowed.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionEnrollControlAllowed { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionLabel { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionLongDescription { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.maxNumberofStudents.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionMaxNumberOfStudents { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.numberofStudents.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionNumberOfStudents { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionShortDescription { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.sourceSectionId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionSourceSectionId { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.timeFrame.begin.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionTimeFrameBegin { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.timeFrame.end.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionTimeFrameEnd { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseSectionTitle { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseTemplateCourseNumber { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseTemplateCredits { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseTemplateLabel { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseTemplateLongDescription { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseTemplateShortDescription { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseTemplateSourcedId { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisCourseTemplateTitle { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.email.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupEmail { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.enrollControl.accept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupEnrollControlAccept { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.enrollControl.Allowed.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupEnrollControlAllowed { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.level.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupGrouptypeLevel { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.scheme.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupGrouptypeScheme { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.typevalue.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupGrouptypeTypevalue { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupLongDescription { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.parentId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupParentId { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupShortDescription { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupSourcedId { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.timeFrame.begin.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupTimeFrameBegin { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.timeFrame.end.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupTimeFrameEnd { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.url.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisGroupUrl { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisLineItemDataSource { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.resultValue.list.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisLineItemResultValueList { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.resultValue.max.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisLineItemResultValueMax { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisLineItemSourcedId { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.type.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisLineItemType { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.type.displayName.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisLineItemTypeDisplayName { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.collectionSourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisMembershipCollectionSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.createdTimestamp.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisMembershipCreatedTimestamp { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisMembershipDataSource { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.personSourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisMembershipPersonSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.role.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisMembershipRole { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisMembershipSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.status.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisMembershipStatus { get; set; }

        /// <summary>
        /// This field contains the country of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.country.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonAddressCountry { get; set; }

        /// <summary>
        /// This field contains the locality of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.locality.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonAddressLocality { get; set; }

        /// <summary>
        /// This field contains the postal code of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.postcode.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonAddressPostCode { get; set; }

        /// <summary>
        /// This field contains the state or province of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.statepr.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonAddressStatePr { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street1.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonAddressStreet1 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street2.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonAddressStreet2 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street3.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonAddressStreet3 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street4.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonAddressStreet4 { get; set; }

        /// <summary>
        /// This field contains the timezone of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.timezone.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonAddressTimezone { get; set; }

        /// <summary>
        /// This field contains the personal email address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.email.personal.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonEmailPersonal { get; set; }

        /// <summary>
        /// This field contains the middle name of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.middle.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonNameMiddle { get; set; }

        /// <summary>
        /// This field contains the name prefix of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.prefix.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonNamePrefix { get; set; }

        /// <summary>
        /// This field contains the name suffix of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.suffix.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonNameSuffix { get; set; }

        /// <summary>
        /// This field contains the home phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.home.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonPhoneHome { get; set; }

        /// <summary>
        /// This field contains the mobile phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.mobile.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonPhoneMobile { get; set; }

        /// <summary>
        /// This field contains the primary phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.primary.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonPhonePrimary { get; set; }

        /// <summary>
        /// This field contains the work phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.work.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonPhoneWork { get; set; }

        /// <summary>
        /// This field contains the SMS number for the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.sms.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonSms { get; set; }

        /// <summary>
        /// This field contains the website address for the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.webaddress.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisPersonWebAddress { get; set; }

        /// <summary>
        /// LIS result vaiable.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Result.createdTimestamp.
        /// Versions: 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisResultCreatedTimestamp { get; set; }

        /// <summary>
        /// LIS result vaiable.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Result.dataSource.
        /// Versions: 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisResultDataSource { get; set; }

        /// <summary>
        /// LIS result vaiable.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Result.resultScore.
        /// Versions: 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisResultResultScore { get; set; }

        /// <summary>
        /// LIS result vaiable.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Result.status.
        /// Versions: 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [NotMapped]
        public string LisResultStatus { get; set; }

        /// <summary>
        /// URL for requests for the tool consumer profile.
        /// This URL should be no more than 1023 characters long and should specify the version of LTI by adding an
        /// lti_version querystring parameter to the URL.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $ToolConsumerProfile.url.
        /// Versions: 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        [NotMapped]
        public string ToolConsumerProfileUrl { get; set; }

        /// <summary>
        /// The username by which the user is known to the TC.
        /// Typically the name entered by the user when they log in.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $User.username.
        /// Versions: 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }

        /// <summary>
        /// A URI describing the user's organisational properties; for example, an ldap:// URI. 
        /// Multiple URIs can be separated using commas.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $User.org.
        /// Versions: 1.2.
        /// </para>
        /// </summary>
        [NotMapped]
        public string UserOrg { get; set; }

        #endregion

        #region Methods

        public void AddCustomParameter(string name, string value)
        {
            // Per the LTI 1.x specs, custom parameter
            // names must be lowercase letters or numbers. Any other
            // character is replaced with an underscore.
            name = Regex.Replace(name, "[^0-9a-zA-Z]", "_");
            if (!name.StartsWith("custom_") && !name.StartsWith("ext_"))
            {
                name = string.Concat("custom_", name);
            }
            // At this point the value may contain custom substitution
            // variables. They will be substituted immediately before launch.
            Parameters.Add(name, value);
        }

        /// <summary>
        /// Add the custom parameters from a string.
        /// </summary>
        /// <param name="parameters"></param>
        public void AddCustomParameters(string parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                var customParams = parameters.Split(new[] { ",", "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var customParam in customParams)
                {
                    var namevalue = customParam.Split(new[] { "=" },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (namevalue.Length == 2)
                    {
                        AddCustomParameter(namevalue[0], namevalue[1]);
                    }
                }
            }
        }

        public LtiRequestViewModel GetLtiRequestViewModel(string consumerSecret)
        {
            var parameters = new NameValueCollection(Parameters);

            // Remove empty/null parameters
            foreach (var key in parameters.AllKeys)
            {
                if (string.IsNullOrWhiteSpace(parameters[key]))
                {
                    parameters.Remove(key);
                }
            }

            // The LTI spec says to include the querystring parameters
            // when calculating the signature base string
            var querystring = HttpUtility.ParseQueryString(Url.Query);
            parameters.Add(querystring);

            // Perform all the custom variable substitutions
            SubstituteCustomVariables(parameters);

            var signature = OAuthUtility.GenerateSignature("POST", Url, parameters, consumerSecret);

            // Now remove the querystring parameters so they are not sent twice
            // (once in the action URL and once in the form data)
            foreach (var key in querystring.AllKeys)
            {
                parameters.Remove(key);
            }

            // Finally fill the LtiRequestBase
            return new LtiRequestViewModel
            {
                Action = Url.ToString(),
                Fields = parameters,
                Signature = signature
            };
        }

        /// <summary>
        /// Get the roles in the LtiRequest as an IList of LtiRoles.
        /// </summary>
        /// <returns></returns>
        public IList<Role> GetRoles()
        {
            var roles = new List<Role>();
            var value = Parameters[LtiConstants.RolesParameter];
            if (string.IsNullOrWhiteSpace(value))
            {
                return roles;
            }
            foreach (var roleName in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Role role;
                if (Enum.TryParse(roleName, true, out role))
                {
                    roles.Add(role);
                }
                else
                {
                    if (LtiConstants.RoleUrns.ContainsKey(roleName))
                    {
                        roles.Add(LtiConstants.RoleUrns[roleName]);
                    }
                }
            }
            return roles;
        }

        public void ParseRequest(HttpRequestBase request)
        {
            ParseRequest(request, false);
        }

        public void ParseRequest(HttpRequestBase request, bool skipValidation)
        {
            HttpMethod = request.HttpMethod;
            Url = request.Url;

            // Launch requests pass parameters as form fields
            Parameters.Add(skipValidation ? request.Unvalidated.Form : request.Form);
        }

        /// <summary>
        /// Set the roles in the LtiRequest from an IList of LtiRoles.
        /// </summary>
        /// <param name="roles">An IList of LtiRoles.</param>
        public void SetRoles(IList<Role> roles)
        {
            Roles = roles.Any() ? string.Join(",", roles.ToList()) : string.Empty;
        }

        /// <summary>
        /// Substitute known custom value tokens. Per the LTI 1.1 spec, unknown
        /// tokens are ignored.
        /// </summary>
        /// <param name="value">Custom value to scan.</param>
        /// <returns>Custom value with the known tokens replaced by their
        /// current value.</returns>
        private string SubstituteCustomVariable(string value)
        {
            // LTI User variables
            value = Regex.Replace(value, "\\$User.id", UserId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.image", UserImage ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.org", UserOrg ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.scope.mentor", RoleScopeMentor ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.username", UserName ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Person variables
            value = Regex.Replace(value, "\\$Person.address.country", LisPersonAddressCountry ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.locality", LisPersonAddressLocality ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.postcode", LisPersonAddressPostCode ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.statepr", LisPersonAddressStatePr ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street1", LisPersonAddressStreet1 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street2", LisPersonAddressStreet2 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street3", LisPersonAddressStreet3 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street4", LisPersonAddressStreet4 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.timezone", LisPersonAddressTimezone ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.email.personal", LisPersonEmailPersonal ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.email.primary", LisPersonEmailPrimary ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.full", LisPersonNameFull ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.family", LisPersonNameFamily ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.given", LisPersonNameGiven ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.middle", LisPersonNameMiddle ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.prefix", LisPersonNamePrefix ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.suffix", LisPersonNameSuffix ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.home", LisPersonPhoneHome ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.mobile", LisPersonPhoneMobile ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.primary", LisPersonPhonePrimary ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.work", LisPersonPhoneWork ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.sms", LisPersonSms ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.sourcedId", LisPersonSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.webaddress", LisPersonWebAddress ?? string.Empty, RegexOptions.IgnoreCase);

            // LTI Context variables
            value = Regex.Replace(value, "\\$Context.id", ContextId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Context.id.history", ContextIdHistory ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Context.org", ContextOrg ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Context.label", ContextLabel ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Context.title", ContextTitle ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Context.type", ContextType.ToString() ?? string.Empty, RegexOptions.IgnoreCase);

            // LTI Resource variables
            value = Regex.Replace(value, "\\$ResourceLink.description", ResourceLinkDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$ResourceLink.id", ResourceLinkId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$ResourceLink.id.history", ResourceLinkIdHistory ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$ResourceLink.title", ResourceLinkTitle ?? string.Empty, RegexOptions.IgnoreCase);

            // LTI Course Template variables
            value = Regex.Replace(value, "\\$CourseTemplate.courseNumber", LisCourseTemplateCourseNumber ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.credits", LisCourseTemplateCredits ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.label", LisCourseTemplateLabel ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.longDescription", LisCourseTemplateLongDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.shortDescription", LisCourseTemplateShortDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.sourcedId", LisCourseTemplateSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.title", LisCourseTemplateTitle ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Course Offering variables
            value = Regex.Replace(value, "\\$CourseOffering.academicSession", LisCourseOfferingAcademicSession ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.courseNumber", LisCourseOfferingCourseNumber ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.credits", LisCourseOfferingCredits ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.label", LisCourseOfferingLabel ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.longDescription", LisCourseOfferingLongDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.shortDescription", LisCourseOfferingShortDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.sourcedId", LisCourseOfferingSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.title", LisCourseOfferingTitle ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Course Section variables
            value = Regex.Replace(value, "\\$CourseSection.courseNumber", LisCourseSectionCourseNumber ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.credits", LisCourseSectionCredits ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.dataSource", LisCourseSectionDataSource ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.dept", LisCourseSectionDept ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.enrollControl.accept", LisCourseSectionEnrollControlAccept ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.enrollControl.allowed", LisCourseSectionEnrollControlAllowed ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.label", LisCourseSectionLabel ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.longDescription", LisCourseSectionLongDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.maxNumberofStudents", LisCourseSectionMaxNumberOfStudents ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.numberofStudents", LisCourseSectionNumberOfStudents ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.shortDescription", LisCourseSectionShortDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.sourceSectionId", LisCourseSectionSourceSectionId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.sourcedId", LisCourseSectionSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.timeFrame.begin", LisCourseSectionTimeFrameBegin ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.timeFrame.end", LisCourseSectionTimeFrameEnd ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.title", LisCourseSectionTitle ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Group vaiables
            value = Regex.Replace(value, "\\$Group.email", LisGroupEmail ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.enrollControl.accept", LisGroupEnrollControlAccept ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.enrollControl.allowed", LisGroupEnrollControlAllowed ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.grouptype.level", LisGroupGrouptypeLevel ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.grouptype.scheme", LisGroupGrouptypeScheme ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.grouptype.typevalue", LisGroupGrouptypeTypevalue ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.longDescription", LisGroupLongDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.parentId", LisGroupParentId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.shortDescription", LisGroupShortDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.sourcedId", LisGroupSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.timeFrame.begin", LisGroupTimeFrameBegin ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.timeFrame.end", LisGroupTimeFrameEnd ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.url", LisGroupUrl ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Membership variables
            value = Regex.Replace(value, "\\$Membership.collectionSourcedId", LisMembershipCollectionSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.createdTimestamp", LisMembershipCreatedTimestamp ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.dataSource", LisMembershipDataSource ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.personSourcedId", LisMembershipPersonSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.role", LisMembershipRole ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.sourcedId", LisMembershipSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.status", LisMembershipStatus ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS LineItem variables
            value = Regex.Replace(value, "\\$LineItem.dataSource", LisLineItemDataSource ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$LineItem.resultValue.list", LisLineItemResultValueList ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$LineItem.resultValue.max", LisLineItemResultValueMax ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$LineItem.sourcedId", LisLineItemSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$LineItem.type", LisLineItemType ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$LineItem.type.displayName", LisLineItemTypeDisplayName ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Result variables
            value = Regex.Replace(value, "\\$Result.createdTimestamp", LisResultCreatedTimestamp ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Result.dataSource", LisResultDataSource ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Result.resultScore", LisResultResultScore ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Result.sourcedId", LisResultSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Result.status", LisResultStatus ?? string.Empty, RegexOptions.IgnoreCase);

            // Tool Consumer Profile
            value = Regex.Replace(value, "\\$ToolConsumerProfile.url", ToolConsumerProfileUrl ?? string.Empty, RegexOptions.IgnoreCase);

            return value;
        }

        /// <summary>
        /// Perform all the custom variable substitutions
        /// </summary>
        private void SubstituteCustomVariables(NameValueCollection parameters)
        {
            foreach (var key in parameters.AllKeys)
            {
                if (key.StartsWith("custom_"))
                {
                    // Per the LTI 1.x specs, custom parameter
                    // names must be lowercase letters or numbers. Any other
                    // character is replaced with an underscore.
                    var value = SubstituteCustomVariable(parameters[key]);
                    parameters[key] = value;
                }
            }
        }

        #endregion
    }

    public class LtiRequestViewModel
    {
        public string Action { get; set; }
        public NameValueCollection Fields { get; set; }
        public string Signature { get; set; }
    }
}
