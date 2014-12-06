
namespace LtiLibrary.ContentItems
{
    public interface IContentItem
    {
        string Id { get; set; }
        ContentItemType Type { get; }
        Image Icon { get; set; }
        string MediaType { get; set; }
        string Text { get; set; }
        Image Thumbnail { get; set; }
        string Title { get; set; }
    }
}