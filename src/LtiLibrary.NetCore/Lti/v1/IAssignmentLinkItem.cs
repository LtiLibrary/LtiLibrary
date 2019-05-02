using LtiLibrary.NetCore.Lis.v2;
using System.Collections.Generic;

namespace LtiLibrary.NetCore.Lti.v1
{
	internal interface IAssignmentLinkItem : IContentItem
	{
		LineItem LineItem { get; set; }
	}
}