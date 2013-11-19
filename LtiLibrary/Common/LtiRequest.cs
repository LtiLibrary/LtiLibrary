using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;

namespace LtiLibrary.Common
{
    public class LtiRequest
    {
        private static readonly Dictionary<string, LtiRoles> Urns = new Dictionary<string, LtiRoles>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Primarily used to build a lookup table of role URNs
        /// </summary>
        static LtiRequest()
        {
            var type = typeof(LtiRoles);
            foreach (LtiRoles ltiRole in Enum.GetValues(type))
            {
                var memInfo = type.GetMember(ltiRole.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(UrnAttribute), false);
                var urn = ((UrnAttribute)attributes[0]).Urn;
                Urns.Add(urn, ltiRole);
            }
        }

        public LtiRequest()
        {
            CustomParameters = new NameValueCollection();
            LaunchPresentationLocale = CultureInfo.CurrentCulture.Name;
            LtiMessageType = LtiConstants.LtiMessageType;
            LtiVersion = LtiConstants.LtiVersion;
            Roles = new List<LtiRoles>();
        }

        /// <summary>
        /// This is an opaque identifier that uniquely identifies the context that contains the link being launched.
        /// <para>
        /// Parameter: context_id.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// A plain text label for the context – intended to fit in a column.
        /// <para>
        /// Parameter: context_label.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        public string ContextLabel { get; set; }

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
        /// A plain text title of the context – it should be about the length of a line.
        /// <para>
        /// Parameter: context_title.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        public string ContextTitle { get; set; }

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
        public LisContextTypes? ContextType { get; set; }

        /// <summary>
        /// A collection of custom parameters in this request.
        /// </summary>
        public NameValueCollection CustomParameters { get; private set; }

        /// <summary>
        /// Gets or sets the CustomParameters in the form of a querystring.
        /// </summary>
        public string CustomParametersAsQuerystring
        {
            get
            {
                var httpValueCollection = HttpUtility.ParseQueryString(string.Empty);
                httpValueCollection.Add(CustomParameters);
                return httpValueCollection.ToString();
            }
            set { CustomParameters = HttpUtility.ParseQueryString(value); }
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
        public string LaunchPresentationLocale { get; set; }

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
        public string LaunchPresentationCssUrl { get; set; }

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
        public PresentationTargets? LaunchPresentationDocumentTarget { get; set; }

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
        public int? LaunchPresentationHeight { get; set; }

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
        public string LaunchPresentationReturnUrl { get; set; }

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
        public int? LaunchPresentationWidth { get; set; }

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
        /// This field contains a LIS course identifier associated with the context of this launch. 
        /// <para>
        /// Parameter: lis_course_offering_sourcedid.
        /// Custom parameter substitution: $CourseOffering.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        public string LisCourseOfferingSourcedId { get; set; }

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
        /// This field contains a LIS course identifier associated with the context of this launch.
        /// <para>
        /// Parameter: lis_course_section_sourcedid.
        /// Custom parameter substitution: $CourseSection.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        public string LisCourseSectionSourcedId { get; set; }

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
        public string LisGroupGroupTypeScheme { get; set; }

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
        public string LisOutcomeServiceUrl { get; set; }

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
        /// Parameter: custom parameter substitution only.
        /// Custom parameter substitution: $Person.email.personal.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonEmailPersonal { get; set; }

        /// <summary>
        /// This field contains the primary email address of the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_contact_email_primary.
        /// Custom parameter substitution: $Person.email.primary.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended unless it is suppressed because of privacy settings.
        /// </para>
        /// </summary>
        public string LisPersonEmailPrimary { get; set; }

        /// <summary>
        /// This field contains the family name of the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_name_family.
        /// Custom parameter substitution: $Person.name.family.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended unless it is suppressed because of privacy settings.
        /// </para>
        /// </summary>
        public string LisPersonNameFamily { get; set; }

        /// <summary>
        /// This field contains the full name of the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_name_full.
        /// Custom parameter substitution: $Person.name.full.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended unless it is suppressed because of privacy settings.
        /// </para>
        /// </summary>
        public string LisPersonNameFull { get; set; }

        /// <summary>
        /// This field contains the given name of the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_name_given.
        /// Custom parameter substitution: $Person.name.given.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended unless it is suppressed because of privacy settings.
        /// </para>
        /// </summary>
        public string LisPersonNameGiven { get; set; }

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
        /// This field contains the LIS identifier for the user account that is performing this launch.
        /// <para>
        /// Parameter: lis_person_sourcedid.
        /// Custom parameter substitution: $Person.sourcedId.
        /// Versions: 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        public string LisPersonSourcedId { get; set; }

        /// <summary>
        /// This field contains the website address for the user account that is performing this launch.
        /// <para>
        /// Parameter: custom parameter substitution only.
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
        public string LisResultSourcedId { get; set; }

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
        /// This indicates that this is a basic launch message.
        /// This allows a TP to accept a number of different LTI message types at the same launch URL.
        /// <para>
        /// Parameter: lti_message_type.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Required.
        /// </para>
        /// </summary>
        public string LtiMessageType { get; set; }

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
        public string LtiVersion { get; set; }

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
        public string ResourceLinkDescription { get; set; }

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
        public string ResourceLinkId { get; set; }

        /// <summary>
        /// A plain text title for the resource. This is the clickable text that appears in the link.
        /// <para>
        /// Parameter: resource_link_title.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        public string ResourceLinkTitle { get; set; }

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
        public IList<LtiRoles> Roles { get; set; }
        public string RolesAsString
        {
            get { return string.Join(",", Roles.ToList()); }
            set
            {
                Roles = new List<LtiRoles>();
                if (string.IsNullOrWhiteSpace(value)) return;
                foreach (var roleName in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    LtiRoles role;
                    if (Enum.TryParse(roleName, true, out role))
                    {
                        Roles.Add(role);
                    }
                    else
                    {
                        if (Urns.ContainsKey(roleName))
                        {
                            Roles.Add(Urns[roleName]);
                        }
                    }
                }
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
        public string RoleScopeMentor { get; set; }

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
        public string ToolConsumerInfoProductFamilyCode { get; set; }

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
        public string ToolConsumerInfoVersion { get; set; }

        /// <summary>
        /// An email contact for the TC instance.
        /// <para>
        /// Parameter: tool_consumer_instance_contact_email.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        public string ToolConsumerInstanceContactEmail { get; set; }

        /// <summary>
        /// This is a plain text user visible field – it should be about the length of a line.
        /// <para>
        /// Parameter: tool_consumer_instance_description.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        public string ToolConsumerInstanceDescription { get; set; }

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
        public string ToolConsumerInstanceGuid { get; set; }

        /// <summary>
        /// This is a plain text user visible field – it should be about the length of a column.
        /// <para>
        /// Parameter: tool_consumer_instance_name.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        public string ToolConsumerInstanceName { get; set; }

        /// <summary>
        /// This is the URL of the consumer instance. This parameter is optional.
        /// <para>
        /// Parameter: tool_consumer_instance_url.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        public string ToolConsumerInstanceUrl { get; set; }

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
        /// The resource URL.
        /// </summary>
        public string Url { get; set; }

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
        public string UserId { get; set; }

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
        public string UserImage { get; set; }

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
    }
}