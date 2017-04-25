using System.Collections.Generic;

namespace LtiLibrary.NetCore.ContentItems
{
    internal interface ILtiLink : IContentItem
    {
        IDictionary<string, string> Custom { get; set; }
    }
}