using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiLibrary.Core.ContentItems
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentItemType
    {
        ContentItem,
        FileItem,
        LtiLink
    }
}
