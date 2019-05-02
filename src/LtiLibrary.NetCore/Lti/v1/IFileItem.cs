using System;

namespace LtiLibrary.NetCore.Lti.v1
{
	internal interface IFileItem : IContentItem
	{
		bool? CopyAdvice { get; set; }
		DateTime? ExpiresAt { get; set; }
	}
}