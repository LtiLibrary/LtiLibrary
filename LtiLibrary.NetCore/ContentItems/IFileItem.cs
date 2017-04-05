using System;

namespace LtiLibrary.NetCore.ContentItems
{
    public interface IFileItem : IContentItem
    {
        bool? CopyAdvice { get; set; }
        DateTime? ExpiresAt { get; set; }
    }
}