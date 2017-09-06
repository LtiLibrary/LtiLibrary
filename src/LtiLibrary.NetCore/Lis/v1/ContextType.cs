using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiLibrary.NetCore.Lis.v1
{
    /// <summary>
    /// Represents IMS LisContextTypes.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContextType
    {
        /// <summary>
        /// A course offering relates to the specific period of time when the course is available.
        /// </summary>
        [Urn("urn:lti:context-type:ims/lis/CourseOffering")]
        CourseOffering,

        /// <summary>
        /// A course section is the specific instance into which students are enrolled and taught.
        /// </summary>
        [Urn("urn:lti:context-type:ims/lis/CourseSection")]
        CourseSection,

        /// <summary>
        /// A course template is the abstract course which is independent of when it is taught.
        /// The course template may have one or more course offerings, each of which may have one or more course sections.
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
