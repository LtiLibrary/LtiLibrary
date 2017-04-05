using System.Collections.Generic;

namespace LtiLibrary.NetCore.ContentItems
{
    public interface ILtiLink : IContentItem
    {
        IDictionary<string, string> Custom { get; set; }
    }
}