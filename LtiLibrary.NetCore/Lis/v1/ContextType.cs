using LtiLibrary.NetCore.Common;

namespace LtiLibrary.NetCore.Lis.v1
{
    /// <summary>
    /// Represents IMS LisContextTypes.
    /// </summary>
    public enum ContextType
    {
        /// <summary>
        /// A course offering that might appear in a course catalog (e.g. ECON 101).
        /// </summary>
        [Urn("urn:lti:context-type:ims/lis/CourseOffering")]
        CourseOffering,

        /// <summary>
        /// An instance of the course (e.g. Miss Marple's Home Room Class, or ECON 101 - Section 1).
        /// </summary>
        [Urn("urn:lti:context-type:ims/lis/CourseSection")]
        CourseSection,

        /// <summary>
        /// The syllabus or curriculum for a course offering.
        /// </summary>
        [Urn("urn:lti:context-type:ims/lis/CourseTemplate")]
        CourseTemplate,

        /// <summary>
        /// A course group within a course section.
        /// </summary>
        [Urn("urn:lti:context-type:ims/lis/Group")]
        Group
    }
}
