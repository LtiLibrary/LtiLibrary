using LtiLibrary.NetCore.Common;

namespace LtiLibrary.NetCore.Lis.v1
{
    /// <summary>
    /// The role of the user within the institution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The user may have multiple system, institutional, and context roles.
    /// </para>
    /// <para>LTI 1.x uses a subset of LIS roles.
    /// See http://www.imsglobal.org/specs/ltiv1p1p1/implementation-guide#toc-31.
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
    public enum InstitutionalRole
    {
        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Administrator")]
        Administrator,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Alumni")]
        Alumni,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Faculty")]
        Faculty,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Guest")]
        Guest,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Instructor")]
        Instructor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Learner")]
        Learner,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Member")]
        Member,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Mentor")]
        Mentor,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/None")]
        None,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Observer")]
        Observer,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Other")]
        Other,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/ProspectiveStudent")]
        ProspectiveStudent,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Staff")]
        Staff,

        /// <summary>
        /// </summary>
        [Urn("urn:lti:instrole:ims/lis/Student")]
        Student,
    }
}