using System.Collections.Generic;

namespace LtiLibrary.NetCore.Lti.v1
{
    internal interface ILtiLink : IContentItem
    {
        IDictionary<string, string> Custom { get; set; }
    }
}