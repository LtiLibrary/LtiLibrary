using System;

namespace LtiLibrary.ContentItems
{
    public interface IFileItem : IContentItem
    {
        bool? CopyAdvice { get; set; }
        DateTime? ExpiresAt { get; set; }
    }
}