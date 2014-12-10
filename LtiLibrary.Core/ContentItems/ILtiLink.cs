using System.Collections.Generic;

namespace LtiLibrary.Core.ContentItems
{
    public interface ILtiLink : IContentItem
    {
        IDictionary<string, string> Custom { get; set; }
    }
}