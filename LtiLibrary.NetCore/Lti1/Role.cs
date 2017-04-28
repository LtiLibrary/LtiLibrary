using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiLibrary.NetCore.Lti1
{
    /// <summary>
    /// Represents IMS roles.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Role
    {
        /// <summary>
        /// An administrator.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator")]
        Administrator,

        /// <summary>
        /// A content developer.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/ContentDeveloper")]
        ContentDeveloper,

        /// <summary>
        /// An instructor or teacher.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Instructor")]
        Instructor,

        /// <summary>
        /// A learner or student.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Learner")]
        Learner,

        /// <summary>
        /// A mentor or parent.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Mentor")]
        Mentor,

        /// <summary>
        /// A teaching assistant.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant")]
        TeachingAssistant
    }
}
