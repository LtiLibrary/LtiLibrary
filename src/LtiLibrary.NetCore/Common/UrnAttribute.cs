using System;

namespace LtiLibrary.NetCore.Common
{
    /// <summary>
    /// Roles in LTI are defined by URNs, although in practice, you can use a nickname such as Learner
    /// instead of urn:lti:role:ims/lis/Learner. This attribute associates each nickname with the
    /// corresponding URN.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class UrnAttribute : Attribute
    {
        /// <summary>
        /// Initialize a new instance of the UrnAttribute class.
        /// </summary>
        /// <param name="urn">The URN associated with the enumerated field value.</param>
        public UrnAttribute(string urn)
        {
            Urn = urn;
        }

        /// <summary>
        /// Get or Set the URN associated with the enumerated field value.
        /// </summary>
        public string Urn { get; set; }
    }
}
