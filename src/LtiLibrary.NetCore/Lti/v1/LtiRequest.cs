using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.OAuth;

namespace LtiLibrary.NetCore.Lti.v1
{
    /// <summary>
    /// Represents an IMS LTI request.
    /// </summary>
    [DataContract]
    public class LtiRequest 
        : OAuthRequest, IBasicLaunchRequest, IOutcomesManagementRequest, IContentItemSelectionRequest, IContentItemSelection
    {
        #region Static Member Data

        // These LTI launch parameters are required
        private static readonly string[] RequiredBasicLaunchParameters =
        {
            LtiConstants.LtiMessageTypeParameter,
            LtiConstants.LtiVersionParameter,
            LtiConstants.ResourceLinkIdParameter
        };

        // These LTI Content Item parameters are required
        private static readonly string[] RequiredContentItemLaunchParameters =
        {
            LtiConstants.AcceptMediaTypesParameter,
            LtiConstants.AcceptPresentationDocumentTargetsParameter,
            LtiConstants.ContentItemReturnUrlParameter,
            LtiConstants.LtiMessageTypeParameter,
            LtiConstants.LtiVersionParameter
        };

        // These LTI Content Item parameters are required
        private static readonly string[] RequiredContentItemResponseParameters =
        {
            LtiConstants.ContentItemPlacementParameter
        };

        // Required Outcomes 2 parameters if any service URLs are specificed
        private static readonly string[] RequiredOutcomes2Parameters =
        {
            LtiConstants.UserIdParameter
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize an empty LtiRequest.
        /// </summary>
        /// <remarks>This is used when extracting an LtiRequest from an HttpRequest and for unit tests.</remarks>
        public LtiRequest() : this(null) {}

        /// <summary>
        /// Initialize a new instanace of the LtiRequest class with the specified message type. This also sets up
        /// the OAuth values such as Nonce and Timestamp.
        /// </summary>
        /// <param name="messageType">The <see cref="LtiMessageType"/> for this request.</param>
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

            if (!messageType.Equals(LtiConstants.LtiOauthBodyHashMessageType))
            {
                // LTI defaults
                HttpMethod = System.Net.Http.HttpMethod.Post.Method;
                LaunchPresentationLocale = CultureInfo.CurrentCulture.Name;
                LtiMessageType = messageType;
                LtiVersion = LtiConstants.LtiVersion;
            }
        }

#endregion

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
                return InternalParameters[LtiConstants.ContextIdParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ContextIdParameter] = value;
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
                return InternalParameters[LtiConstants.ContextIdHistoryParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ContextIdHistoryParameter] = value;
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
                return InternalParameters[LtiConstants.ContextLabelParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ContextLabelParameter] = value;
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
                return InternalParameters[LtiConstants.ContextTitleParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ContextTitleParameter] = value;
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
        [DataMember(Name = LtiConstants.ContextTypeParameter)]
        public ContextType? ContextType
        {
            get
            {
                return Enum.TryParse(InternalParameters[LtiConstants.ContextTypeParameter], out ContextType contextType)
                   ? contextType
                   : default(ContextType?);
            }
            set
            {
                InternalParameters[LtiConstants.ContextTypeParameter] = Convert.ToString(value);
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
                return InternalParameters[LtiConstants.LaunchPresentationCssUrlParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LaunchPresentationCssUrlParameter] = value;
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
                return Enum.TryParse(InternalParameters[LtiConstants.LaunchPresentationDocumentTargetParameter], out DocumentTarget presentationTarget)
                   ? presentationTarget
                   : default(DocumentTarget?);
            }
            set
            {
                InternalParameters[LtiConstants.LaunchPresentationDocumentTargetParameter] = Convert.ToString(value);
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
                return int.TryParse(InternalParameters[LtiConstants.LaunchPresentationHeightParameter], out var value)
                    ? value
                    : default(int?);
            }
            set
            {
                InternalParameters[LtiConstants.LaunchPresentationHeightParameter] = Convert.ToString(value);
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
                return InternalParameters[LtiConstants.LaunchPresentationLocaleParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LaunchPresentationLocaleParameter] = value;
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
                return InternalParameters[LtiConstants.LaunchPresentationReturnUrlParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LaunchPresentationReturnUrlParameter] = value;
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
        [DataMember(Name = LtiConstants.LaunchPresentationWidthParameter)]
        public int? LaunchPresentationWidth
        {
            get
            {
                return int.TryParse(InternalParameters[LtiConstants.LaunchPresentationWidthParameter], out var value)
                    ? value
                    : default(int?);
            }
            set
            {
                InternalParameters[LtiConstants.LaunchPresentationWidthParameter] = Convert.ToString(value);
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
                return InternalParameters[LtiConstants.LisCourseOfferingSourcedIdParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LisCourseOfferingSourcedIdParameter] = value;
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
                return InternalParameters[LtiConstants.LisCourseSectionSourceIdParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LisCourseSectionSourceIdParameter] = value;
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
                return InternalParameters[LtiConstants.LisPersonContactEmailPrimaryParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LisPersonContactEmailPrimaryParameter] = value;
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
                return InternalParameters[LtiConstants.LisPersonNameFamilyParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LisPersonNameFamilyParameter] = value;
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
                return InternalParameters[LtiConstants.LisPersonNameFullParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LisPersonNameFullParameter] = value;
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
                return InternalParameters[LtiConstants.LisPersonNameGivenParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LisPersonNameGivenParameter] = value;
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
                return InternalParameters[LtiConstants.LisPersonSourcedIdParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LisPersonSourcedIdParameter] = value;
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
                return InternalParameters[LtiConstants.LtiMessageTypeParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LtiMessageTypeParameter] = value;
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
                return InternalParameters[LtiConstants.LtiVersionParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LtiVersionParameter] = value;
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
                return InternalParameters[LtiConstants.RolesParameter];
            }
            set
            {
                InternalParameters[LtiConstants.RolesParameter] = value;
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
                return InternalParameters[LtiConstants.ToolConsumerInfoProductFamilyCodeParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ToolConsumerInfoProductFamilyCodeParameter] = value;
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
                return InternalParameters[LtiConstants.ToolConsumerInfoVersionParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ToolConsumerInfoVersionParameter] = value;
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
                return InternalParameters[LtiConstants.ToolConsumerInstanceContactEmailParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ToolConsumerInstanceContactEmailParameter] = value;
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
                return InternalParameters[LtiConstants.ToolConsumerInstanceDescriptionParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ToolConsumerInstanceDescriptionParameter] = value;
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
                return InternalParameters[LtiConstants.ToolConsumerInstanceGuidParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ToolConsumerInstanceGuidParameter] = value;
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
                return InternalParameters[LtiConstants.ToolConsumerInstanceNameParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ToolConsumerInstanceNameParameter] = value;
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
                return InternalParameters[LtiConstants.ToolConsumerInstanceUrlParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ToolConsumerInstanceUrlParameter] = value;
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
                return InternalParameters[LtiConstants.UserIdParameter];
            }
            set
            {
                InternalParameters[LtiConstants.UserIdParameter] = value;
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
                return InternalParameters[LtiConstants.UserImageParameter];
            }
            set
            {
                InternalParameters[LtiConstants.UserImageParameter] = value;
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
                return InternalParameters[LtiConstants.ResourceLinkDescriptionParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ResourceLinkDescriptionParameter] = value;
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
                return InternalParameters[LtiConstants.ResourceLinkIdParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ResourceLinkIdParameter] = value;
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
                return InternalParameters[LtiConstants.ResourceLinkIdHistoryParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ResourceLinkIdHistoryParameter] = value;
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
                return InternalParameters[LtiConstants.ResourceLinkTitleParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ResourceLinkTitleParameter] = value;
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
                return InternalParameters[LtiConstants.RoleScopeMentorParameter];
            }
            set
            {
                InternalParameters[LtiConstants.RoleScopeMentorParameter] = value;
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
                return InternalParameters[LtiConstants.LisOutcomeServiceUrlParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LisOutcomeServiceUrlParameter] = value;
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
                return InternalParameters[LtiConstants.LisResultSourcedIdParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LisResultSourcedIdParameter] = value;
            }
        }

        #endregion

        #region IContentItemRequest Parameters

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
                return InternalParameters[LtiConstants.AcceptMediaTypesParameter];
            }
            set
            {
                InternalParameters[LtiConstants.AcceptMediaTypesParameter] = value;
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
                if (string.IsNullOrEmpty(InternalParameters[LtiConstants.AcceptMultipleParameter]))
                {
                    return null;
                }

                return bool.TryParse(InternalParameters[LtiConstants.AcceptMultipleParameter], out var value) && value;
            }
            set
            {
                InternalParameters[LtiConstants.AcceptMultipleParameter] = value.HasValue ? value.ToString().ToLowerInvariant() : null;
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
                return InternalParameters[LtiConstants.AcceptPresentationDocumentTargetsParameter];
            }
            set
            {
                InternalParameters[LtiConstants.AcceptPresentationDocumentTargetsParameter] = value;
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
                if (string.IsNullOrEmpty(InternalParameters[LtiConstants.AcceptUnsignedParameter]))
                {
                    return null;
                }

                return bool.TryParse(InternalParameters[LtiConstants.AcceptUnsignedParameter], out var value) && value;
            }
            set
            {
                InternalParameters[LtiConstants.AcceptUnsignedParameter] = value.HasValue ? value.ToString().ToLowerInvariant() : null;
            }
        }

        /// <summary>
        /// This indicates whether any content items returned by the TP would be automatically persisted 
        /// without any option for the user to cancel the operation.  This parameter is optional; when 
        /// omitted  a value of false should be assumed (i.e. items returned may not be persisted at the TC end).
        /// </summary>
        [DataMember(Name = LtiConstants.AutoCreateParameter)]
        public bool? AutoCreate
        {
            get
            {
                if (string.IsNullOrEmpty(InternalParameters[LtiConstants.AutoCreateParameter]))
                {
                    return null;
                }

                return bool.TryParse(InternalParameters[LtiConstants.AutoCreateParameter], out var value) && value;
            }
            set
            {
                InternalParameters[LtiConstants.AutoCreateParameter] = value.HasValue ? value.ToString().ToLowerInvariant() : null;
            }
        }

        /// <summary>
        /// This indicates whether the TC supports the feature for confirming the persistence of content items 
        /// received.  When a value of true is passed, the TP may include a confirm_url parameter in its return 
        /// message containing the endpoint to which a confirmation request should be sent (see below).  If the 
        /// TP does not support this feature or does not wish to receive a confirmation it may just omit the 
        /// confirm_url parameter from its return message.  This option may be used even when the auto_create 
        /// parameter is set to true if a TC is willing to offer the additional reassurance that items have been 
        /// persisted, or to allow a TP to be notified of resource link IDs for any LTI links which have been created.
        /// </summary>
        [DataMember(Name = LtiConstants.CanConfirmParameter)]
        public bool? CanConfirm
        {
            get
            {
                if (string.IsNullOrEmpty(InternalParameters[LtiConstants.CanConfirmParameter]))
                {
                    return null;
                }

                return bool.TryParse(InternalParameters[LtiConstants.CanConfirmParameter], out var value) && value;
            }
            set
            {
                InternalParameters[LtiConstants.CanConfirmParameter] = value.HasValue ? value.ToString().ToLowerInvariant() : null;
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
                return InternalParameters[LtiConstants.ContentItemReturnUrlParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ContentItemReturnUrlParameter] = value;
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
                return InternalParameters[LtiConstants.ContentItemDataParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ContentItemDataParameter] = value;
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
                return InternalParameters[LtiConstants.ContentItemTextParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ContentItemTextParameter] = value;
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
                return InternalParameters[LtiConstants.ContentItemTitleParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ContentItemTitleParameter] = value;
            }
        }

        #endregion

        #region IContentItemSelection Parameters

        /// <summary>
        /// If the original content-item message received included a can_confirm parameter with a value of true, 
        /// then this parameter may be included in the response to the TC.  Its value should be the endpoint for 
        /// a ContentItem service request (see below).  Any content item for which a confirmation is required 
        /// should include an @id element so that it can be identified in the service request received by the TP.
        /// </summary>
        [DataMember(Name = LtiConstants.ConfirmUrlParameter)]
        public string ConfirmUrl
        {
            get
            {
                return InternalParameters[LtiConstants.ConfirmUrlParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ConfirmUrlParameter] = value;
            }
        }

        /// <summary>
        /// The value of this parameter should be a JSON array containing details of each of the items selected. 
        /// If no items have been selected this parameter may contain an empty array or be omitted.
        /// </summary>
        [DataMember(Name = LtiConstants.ContentItemPlacementParameter)]
        public string ContentItems
        {
            get
            {
                return InternalParameters[LtiConstants.ContentItemPlacementParameter];
            }
            set
            {
                InternalParameters[LtiConstants.ContentItemPlacementParameter] = value;
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
                return InternalParameters[LtiConstants.LtiErrorLogParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LtiErrorLogParameter] = value;
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
                return InternalParameters[LtiConstants.LtiErrorMsgParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LtiErrorMsgParameter] = value;
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
                return InternalParameters[LtiConstants.LtiLogParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LtiLogParameter] = value;
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
                return InternalParameters[LtiConstants.LtiMsgParameter];
            }
            set
            {
                InternalParameters[LtiConstants.LtiMsgParameter] = value;
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
        public string ContextOrg { get; set; }

        /// <summary>
        /// Endpoint for Outcomes-2 LineItem service
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.url.
        /// Versions: 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        public string LineItemServiceUrl { get; set; }

        /// <summary>
        /// Endpoint for Outcomes-2 LineItem collection service
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItems.url.
        /// Versions: 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        public string LineItemsServiceUrl { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.academicSession.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingAcademicSession { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingCourseNumber { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingCredits { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingLabel { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingLongDescription { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingShortDescription { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingTitle { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionCourseNumber { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionCredits { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionDataSource { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.dept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionDept { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.enrollControl.accept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionEnrollControlAccept { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.enrollControl.allowed.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionEnrollControlAllowed { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionLabel { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionLongDescription { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.maxNumberofStudents.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionMaxNumberOfStudents { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.numberofStudents.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionNumberOfStudents { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionShortDescription { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.sourceSectionId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionSourceSectionId { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.timeFrame.begin.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionTimeFrameBegin { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.timeFrame.end.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionTimeFrameEnd { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionTitle { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateCourseNumber { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateCredits { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateLabel { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateLongDescription { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateShortDescription { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateSourcedId { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateTitle { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.email.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupEmail { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.enrollControl.accept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupEnrollControlAccept { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.enrollControl.Allowed.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupEnrollControlAllowed { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.level.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupGrouptypeLevel { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.scheme.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupGrouptypeScheme { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.typevalue.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupGrouptypeTypevalue { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupLongDescription { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.parentId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupParentId { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupShortDescription { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupSourcedId { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.timeFrame.begin.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupTimeFrameBegin { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.timeFrame.end.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupTimeFrameEnd { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.url.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupUrl { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisLineItemDataSource { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.resultValue.list.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisLineItemResultValueList { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.resultValue.max.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisLineItemResultValueMax { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisLineItemSourcedId { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.type.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisLineItemType { get; set; }

        /// <summary>
        /// LIS LineItem variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $LineItem.type.displayName.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisLineItemTypeDisplayName { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.collectionSourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipCollectionSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.createdTimestamp.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipCreatedTimestamp { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipDataSource { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.personSourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipPersonSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.role.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipRole { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.status.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipStatus { get; set; }

        /// <summary>
        /// This field contains the country of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.country.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressCountry { get; set; }

        /// <summary>
        /// This field contains the locality of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.locality.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressLocality { get; set; }

        /// <summary>
        /// This field contains the postal code of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.postcode.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressPostCode { get; set; }

        /// <summary>
        /// This field contains the state or province of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.statepr.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStatePr { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street1.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStreet1 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street2.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStreet2 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street3.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStreet3 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street4.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStreet4 { get; set; }

        /// <summary>
        /// This field contains the timezone of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.timezone.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressTimezone { get; set; }

        /// <summary>
        /// This field contains the personal email address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.email.personal.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonEmailPersonal { get; set; }

        /// <summary>
        /// This field contains the middle name of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.middle.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonNameMiddle { get; set; }

        /// <summary>
        /// This field contains the name prefix of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.prefix.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonNamePrefix { get; set; }

        /// <summary>
        /// This field contains the name suffix of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.suffix.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonNameSuffix { get; set; }

        /// <summary>
        /// This field contains the home phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.home.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonPhoneHome { get; set; }

        /// <summary>
        /// This field contains the mobile phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.mobile.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonPhoneMobile { get; set; }

        /// <summary>
        /// This field contains the primary phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.primary.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonPhonePrimary { get; set; }

        /// <summary>
        /// This field contains the work phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.work.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonPhoneWork { get; set; }

        /// <summary>
        /// This field contains the SMS number for the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.sms.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonSms { get; set; }

        /// <summary>
        /// This field contains the website address for the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.webaddress.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
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
        public string LisResultStatus { get; set; }

        /// <summary>
        /// Endpoint for Outcomes-2 LISResult service
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Result.url.
        /// Versions: 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        public string ResultServiceUrl { get; set; }

        /// <summary>
        /// Endpoint for Outcomes-2 LISResult collection service
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Results.url.
        /// Versions: 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        public string ResultsServiceUrl { get; set; }

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
        public string UserOrg { get; set; }

        #endregion

        #region Properties

        public IList<KeyValuePair<string, string>> Parameters
        {
            get
            {
                var parameters = new List<KeyValuePair<string, string>>();
                foreach (var name in InternalParameters.AllKeys)
                {
                    var values = InternalParameters.GetValues(name);
                    if (values != null)
                    {
                        foreach (var value in values)
                        {
                            parameters.Add(new KeyValuePair<string, string>(name, value));
                        }
                    }
                }

                return parameters;
            }
        }

        #endregion

        #region Methods

        public void AddParameter(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{nameof(name)} cannot be null, empty, or consist only of whitespace characters.");
            }

            if (value == null)
            {
                throw new ArgumentException($"{nameof(value)} cannot be null.");
            }

            // Trim any whitespace that surrounds the name
            name = name.Trim();

            // Trim any whitespace that surrounds the value
            value = value.Trim();

            // At this point the value may contain custom substitution
            // variables. They will be substituted immediately before launch.
            InternalParameters.Add(name, value);
        }

        /// <summary>
        /// Add custom parameters to the LtiRequest. The name/value pairs can be
        /// separated by commas, semicolons, ampersands, or line breaks. If you need
        /// to include one of those characters in the value, use <see cref="AddCustomParameter"/>.
        /// </summary>
        /// <remarks>
        /// Use <see cref="CustomParameters"/> to replace the custom paramters in the
        /// LtiRequest.
        /// </remarks>
        public void AddCustomParameters(string parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                var customParams = parameters.Split(new[] { ",", ";", "\r\n", "\n", "&" },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var customParam in customParams)
                {
                    var namevalue = customParam.Split(new[] { "=" },
                        StringSplitOptions.None);
                    if (namevalue.Length == 2)
                    {
                        AddCustomParameter(namevalue[0], namevalue[1]);
                    }
                }
            }
        }

        /// <summary>
        /// Add a custom parameter to the LtiRequest.
        /// </summary>
        /// <param name="name">The name of the custom parameter. The name will be converted to meet the
        /// LTI spec (e.g. lowercase letters, numbers, and underscore only) and if the name
        /// does not start with "custom_" or "ext_", then "custom_" will be prepended.</param>
        /// <param name="value">The value of the custom parameter.</param>
        public void AddCustomParameter(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{nameof(name)} cannot be null, empty, or consist only of whitespace characters.");
            }

            if (value == null)
            {
                throw new ArgumentException($"{nameof(value)} cannot be null.");
            }

            // Trim any whitespace that surrounds the name
            name = name.Trim();

            // Per the LTI 1.x specs, custom parameter
            // names must be lowercase letters or numbers. Any other
            // character is replaced with an underscore.
            name = Regex.Replace(name, "[^0-9a-zA-Z]", "_").ToLowerInvariant();
            if (!name.StartsWith("custom_") && !name.StartsWith("ext_"))
            {
                name = string.Concat("custom_", name);
            }

            AddParameter(name, value);
        }

        /// <summary>
        /// Throws an <see cref="LtiException"/> if any required parameters are missing.
        /// </summary>
        public void CheckForRequiredLtiParameters()
        {
            if (HttpMethod == null || !HttpMethod.Equals(System.Net.Http.HttpMethod.Post.Method))
            {
                throw new LtiException($"Invalid HTTP method: {HttpMethod??"null"}.");
            }

            if (Url == null)
            {
                throw new LtiException("Missing parameter(s): Url.");
            }

            // Make sure the request contains all the required parameters
            RequireAllOf(RequiredOauthParameters);
            switch (LtiMessageType)
            {
                case LtiConstants.BasicLaunchLtiMessageType:
                {
                    RequireAllOf(RequiredBasicLaunchParameters);
                    break;
                }
                case LtiConstants.ContentItemSelectionRequestLtiMessageType:
                {
                    RequireAllOf(RequiredContentItemLaunchParameters);
                    if (!Uri.IsWellFormedUriString(ContentItemReturnUrl, UriKind.RelativeOrAbsolute))
                    {
                        throw new LtiException($"Invalid {LtiConstants.ContentItemReturnUrlParameter}: {ContentItemReturnUrl}.");
                    }
                    break;
                }
                case LtiConstants.ContentItemSelectionLtiMessageType:
                {
                    RequireAllOf(RequiredContentItemResponseParameters);
                    break;
                }
                default:
                    throw new LtiException($"Invalid {LtiConstants.LtiMessageTypeParameter}: {LtiMessageType}.");
            }

            // If the request is configured to support Outcomes 2.0, make sure user_id is specified
            if (!string.IsNullOrWhiteSpace(LineItemServiceUrl) || !string.IsNullOrWhiteSpace(LineItemsServiceUrl)
                || !string.IsNullOrWhiteSpace(ResultServiceUrl) || !string.IsNullOrWhiteSpace(ResultsServiceUrl))
            {
                RequireAllOf(RequiredOutcomes2Parameters);
            }
        }

        /// <summary>
        /// Gets or sets the custom_ and ext_ parameters of the <see cref="OAuthRequest"/>. Setting the value
        /// will replace all the custom_ and ext_ parameters in the request. Use <see cref="Lti.v1.LtiRequest.AddCustomParameters"/>
        /// to add parameters. Getting the value will return all the custom_ and ext_ parameters in querystring format.
        /// </summary>
        [DataMember]
        public string CustomParameters
        {
            get
            {
                var customParameters = new UrlEncodingParser("");
                foreach (var pair in Parameters)
                {
                    if (pair.Key.StartsWith("custom_") || pair.Key.StartsWith("ext_"))
                    {
                        customParameters.Add(pair.Key, pair.Value);
                    }
                }
                return customParameters.Count == 0 ? null : customParameters.ToString();
            }
            set
            {
                // Remove all the existing custom parameters
                foreach (var name in InternalParameters.AllKeys)
                {
                    if (name.StartsWith("custom_") || name.StartsWith("ext_"))
                    {
                        InternalParameters.Remove(name);
                    }
                }
                // Add the new custom_ and _ext parameters
                AddCustomParameters(value);
            }
        }

        /// <summary>
        /// Get the roles in the LtiRequest as an IList of ContextRole, InstitutionalRole, and SystemRole enums.
        /// </summary>
        public IList<Enum> GetRoles()
        {
            var roles = new List<Enum>();
            if (string.IsNullOrWhiteSpace(Roles))
            {
                return roles;
            }
            foreach (var roleValue in Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Enum.TryParse(roleValue, true, out ContextRole role))
                {
                    roles.Add(role);
                }
                else
                {
                    if (LtiConstants.RoleUrns.ContainsKey(roleValue))
                    {
                        roles.Add(LtiConstants.RoleUrns[roleValue]);
                    }
                }
            }
            return roles;
        }

        /// <summary>
        /// Throws an <see cref="LtiException"/> if this instance of the LtiRequest does not have a
        /// value set for any of the named parameters.
        /// </summary>
        /// <param name="parameters">The list of parameter names to check.</param>
        public void RequireAllOf(IEnumerable<string> parameters)
        {
            var missing = parameters.Where(parameter => string.IsNullOrWhiteSpace(InternalParameters[parameter])).ToList();

            if (missing.Count > 0)
            {
                var missingParameters = string.Join(", ", missing.ToArray());
                throw new LtiException($"Missing parameter(s): {missingParameters}.");
            }
        }

        /// <summary>
        /// Set the roles in the LtiRequest from an IList of ContextRole, InstitutionalRole, and SystemRole.
        /// </summary>
        public void SetRoles(IList<Enum> roleEnums)
        {
            var roles = new List<string>();
            foreach (var roleEnum in roleEnums)
            {
                if (roleEnum is ContextRole)
                {
                    roles.Add(roleEnum.ToString());
                }
                else
                {
                    var urn = roleEnum.GetUrn();
                    if (!string.IsNullOrEmpty(urn))
                    {
                        roles.Add(urn);
                    }
                }
            }
            Roles = roles.Any() ? string.Join(",", roles) : string.Empty;
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
            value = Regex.Replace(value, "\\$Context.type", ContextType.ToString(), RegexOptions.IgnoreCase);

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

            // Outcomes-2
            value = Regex.Replace(value, "\\$LineItem.url", LineItemServiceUrl ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$LineItems.url", LineItemsServiceUrl ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Result.url", ResultServiceUrl ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Results.url", ResultsServiceUrl ?? string.Empty, RegexOptions.IgnoreCase);

            return value;
        }

        private void SubstituteCustomVariables(NameValueCollection parameters)
        {
            foreach (var name in parameters.AllKeys)
            {
                if (name.StartsWith("custom_"))
                {
                    var oldValues = parameters.GetValues(name);
                    var newValues = new List<string>();
                    InternalParameters.Remove(name);

                    if (oldValues != null)
                    {
                        foreach (var oldValue in oldValues)
                        {
                            // Per the LTI 1.x specs, custom parameter
                            // names must be lowercase letters or numbers. Any other
                            // character is replaced with an underscore.
                            newValues.Add(SubstituteCustomVariable(oldValue));
                        }
                    }

                    foreach (var newValue in newValues)
                    {
                        AddCustomParameter(name, newValue);
                    }
                }
            }
        }

        private void SubstituteCustomVariables()
        {
            // Create a copy of the parameters (getters should not change the object and this
            // getter changes the parameters to eliminate empty/null values and make custom
            // parameter substitutions)
            var parameters = new NameValueCollection(InternalParameters);

            // Remove empty/null parameters
            foreach (var key in parameters.AllKeys.Where(key => string.IsNullOrWhiteSpace(parameters[key])))
            {
                parameters.Remove(key);
            }

            // Perform the custom variable substitutions
            SubstituteCustomVariables(parameters);
        }

        /// <summary>
        /// Checks for required Lti parameters, substitutes custom variables, returns the OAuth signature.
        /// </summary>
        /// <param name="secret"></param>
        /// <returns>The OAuth signature for the request.</returns>
        /// <remarks>Throws an LtiException if the request is missing required parameters.</remarks>
        public string SubstituteCustomVariablesAndGenerateSignature(string secret)
        {
            CheckForRequiredLtiParameters();
            SubstituteCustomVariables();
            return GenerateSignature(secret);
        }

        #endregion
    }
}
