using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v1
{
    /// <summary>
    /// The role of the user within a given context. Sub-roles are rarely
    /// used (e.g. Learner/GuestLearner).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The role is one of the three main data items which is passed from the Tool Consumer
    /// when a user launches a Tool (the other two items are context and user).
    /// The role represents the level of privilege a user has been given within a context in a Tool Consumer. 
    /// Typical roles are learner, instructor and administrator.  
    /// A Tool may use the role to determine the level of access to give a user.
    /// </para>
    /// <para>LTI 1.x uses a subset of LIS roles.
    /// See http://www.imsglobal.org/specs/ltiv1p1p1/implementation-guide#toc-32.
    /// </para>
    /// <para>
    /// Within an LTI request, if the namespace for a role is ommitted, then
    /// the context role namespace (urn:lti:rule:ims/lis/) is assumed. In practice, 
    /// consumers typically take advantage of this an omit the namespace for context roles.
    /// For example,  they will send "Learner" rather than "urn:lti:role:ims/lis/Learner".
    /// </para>
    /// <para>
    /// Some consumers (including consumers using previous versions of this
    /// library) also omit the namespace for non-context roles. While this is
    /// not technically correct, you should be aware of that behavior and 
    /// be ready to accept those non-context roles (e.g. Student).
    /// </para>
    /// </remarks>
    [JsonConverter(typeof(JsonLdContextRoleConverter))]
    public enum ContextRole
    {
        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator")]
        Administrator,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator/Administrator")]
        AdministratorAdministrator,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator/Developer")]
        AdministratorDeveloper,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator/ExternalDeveloper")]
        AdministratorExternalDeveloper,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator/ExternalSupport")]
        AdministratorExternalSupport,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator/ExternalSystemAdministrator")]
        AdministratorExternalSystemAdministrator,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator/Support")]
        AdministratorSupport,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator/SystemAdministrator")]
        AdministratorSystemAdministrator,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/ContentDeveloper")]
        ContentDeveloper,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/ContentDeveloper/ContentDeveloper")]
        ContentDeveloperContentDeveloper,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/ContentDeveloper/ContentExpert")]
        ContentDeveloperContentExpert,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/ContentDeveloper/ExternalContentExpert")]
        ContentDeveloperExternalContentExpert,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/ContentDeveloper/Librarian")]
        ContentDeveloperLibrarian,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Instructor")]
        Instructor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Instructor/ExternalInstructor")]
        InstructorExternalInstuctor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Instructor/GuestInstructor")]
        InstructorGuestInstuctor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Instructor/Lecturer")]
        InstructorLecturer,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Instructor/PrimaryInstructor")]
        InstructorPrimaryInstuctor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Learner")]
        Learner,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Learner/ExternalLearner")]
        LearnerExternalLearner,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Learner/GuestLearner")]
        LearnerGuestLearner,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Learner/Instructor")]
        LearnerInstructor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Learner/Learner")]
        LearnerLearner,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Learner/NonCreditLearner")]
        LearnerNonCreditLearner,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Manager")]
        Manager,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Manager/AreaManager")]
        ManagerAreaManager,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Manager/CourseCoordinator")]
        ManagerCourseCoordinator,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Manager/ExternalObserver")]
        ManagerExternalObserver,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Manager/Observer")]
        ManagerObserver,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Member")]
        Member,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Member/Member")]
        MemberMember,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Mentor")]
        Mentor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Mentor/Advisor")]
        MentorAdviser,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Mentor/Auditor")]
        MentorAuditor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Mentor/Mentor")]
        MentorMentor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Mentor/Reviewer")]
        MentorReviewer,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Mentor/Tutor")]
        MentorTutor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant")]
        TeachingAssistant,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant/Grader")]
        TeachingAssistantGrader,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant/TeachingAssistant")]
        TeachingAssistantTeachingAssistant,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant/TeachingAssistantSection")]
        TeachingAssistantTeachingAssistantSection,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant/TeachingAssistantSectionAssociation")]
        TeachingAssistantTeachingAssistantSectionAssociation,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant/TeachingAssistantOffering")]
        TeachingAssistantTeachingAssistantOffering,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant/TeachingAssistantTemplate")]
        TeachingAssistantTeachingAssistantTempltae,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant/TeachingAssistantGroup")]
        TeachingAssistantTeachingAssistantGroup,
    }
}