using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v1
{
    /// <summary>
    /// The role of the user within the system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The user may have multiple system, institutional, and context roles.
    /// </para>
    /// <para>LTI 1.x uses a subset of LIS roles.
    /// See http://www.imsglobal.org/specs/ltiv1p1p1/implementation-guide#toc-30.
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
    public enum SystemRole
    {
        /// <summary>
        /// </summary>
        [Urn("urn:lti:sysrole:ims/lis/AccountAdmin")]
        AccountAdmin,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:sysrole:ims/lis/Administrator")]
        Administrator,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:sysrole:ims/lis/Creator")]
        Creator,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:sysrole:ims/lis/None")]
        None,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:sysrole:ims/lis/SysAdmin")]
        SysAdmin,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:sysrole:ims/lis/SysSupport")]
        SysSupport,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:sysrole:ims/lis/User")]
        User,
    }
}