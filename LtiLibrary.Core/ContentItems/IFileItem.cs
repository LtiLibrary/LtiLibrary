using System;

namespace LtiLibrary.Core.ContentItems
{
    public interface IFileItem : IContentItem
    {
        bool? CopyAdvice { get; set; }
        DateTime? ExpiresAt { get; set; }
    }
}