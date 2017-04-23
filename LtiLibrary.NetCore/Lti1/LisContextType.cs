namespace LtiLibrary.NetCore.Lti1
{
    /// <summary>
    /// Represents IMS LisContextTypes.
    /// </summary>
    public enum LisContextType
    {
        /// <summary>
        /// A course offering that might appear in a course catalog (e.g. ECON 101).
        /// </summary>
        CourseOffering,

        /// <summary>
        /// An instance of the course (e.g. Miss Marple's Home Room Class, or ECON 101 - Section 1).
        /// </summary>
        CourseSection,

        /// <summary>
        /// The syllabus or curriculum for a course offering.
        /// </summary>
        CourseTemplate,

        /// <summary>
        /// A course group within a course section.
        /// </summary>
        Group
    }
}
