using System.Runtime.Serialization;

namespace LtiLibrary.ContentItems
{
    [DataContract]
    public class ContentItemsServiceResponse
    {
        [DataMember(Name = "content_items")]
        public ContentItemPlacementResponse ContentItems { get; set; }

        [DataMember(Name = "data")]
        public string Data { get; set; }
    }
}
