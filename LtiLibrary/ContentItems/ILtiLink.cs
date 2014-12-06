using System.Collections.Generic;

namespace LtiLibrary.ContentItems
{
    public interface ILtiLink : IContentItem
    {
        IDictionary<string, string> Custom { get; set; }
    }
}