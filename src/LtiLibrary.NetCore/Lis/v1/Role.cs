using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v1
{
    /// <summary>
    /// Represents IMS roles.
    /// </summary>
    [JsonConverter(typeof(RoleConverter))]
    public enum Role
    {
        /// <summary>
        /// An administrator.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Administrator")]
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Administrator")]
        Administrator,

        /// <summary>
        /// A content developer.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/ContentDeveloper")]
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#ContentDeveloper")]
        ContentDeveloper,

        /// <summary>
        /// An instructor or teacher.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Instructor")]
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Instructor")]
        Instructor,

        /// <summary>
        /// A learner or student.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Learner")]
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Learner")]
        Learner,

        /// <summary>
        /// A mentor or parent.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/Mentor")]
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Mentor")]
        Mentor,

        /// <summary>
        /// A teaching assistant.
        /// </summary>
        [Urn("urn:lti:role:ims/lis/TeachingAssistant")]
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#TeachingAssistant")]
        TeachingAssistant
    }
}
