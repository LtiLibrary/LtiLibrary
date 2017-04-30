
using LtiLibrary.NetCore.Common;

namespace LtiLibrary.NetCore.Lti.v1
{
    internal interface IContentItem : IJsonLdObject
    {
        Image Icon { get; set; }
        string MediaType { get; set; }
        ContentItemPlacement PlacementAdvice { get; set; }
        string Text { get; set; }
        Image Thumbnail { get; set; }
        string Title { get; set; }
        string Url { get; set; }
    }
}