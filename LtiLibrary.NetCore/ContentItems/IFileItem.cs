using System;

namespace LtiLibrary.NetCore.ContentItems
{
    internal interface IFileItem : IContentItem
    {
        bool? CopyAdvice { get; set; }
        DateTime? ExpiresAt { get; set; }
    }
}