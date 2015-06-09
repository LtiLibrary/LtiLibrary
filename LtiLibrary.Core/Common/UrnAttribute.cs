using System;

namespace LtiLibrary.Core.Common
{
    /// <summary>
    /// Roles in LTI are defined by URNs, although in practice, you can use a nickname such as Learner
    /// instead of urn:lti:role:ims/lis/Learner. This attribute associates each nickname with the
    /// corresponding URN.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class UrnAttribute : Attribute
    {
        public UrnAttribute(string urn)
        {
            Urn = urn;
        }

        public string Urn { get; set; }
    }
}
