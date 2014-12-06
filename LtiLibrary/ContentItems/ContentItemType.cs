using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiLibrary.ContentItems
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentItemType
    {
        ContentItem,
        FileItem,
        LtiLink
    }
}
