using System.Runtime.Serialization;

namespace LtiLibrary.ContentItems
{
    [DataContract]
    public class Image
    {
        /// <summary>
        /// The URI that identifies this Image instance.
        /// </summary>
        [DataMember(Name = "@id")]
        public string Id { get; set; }

        /// <summary>
        /// The height in pixels of the image.
        /// </summary>
        [DataMember(Name = "height")]
        public int? Height { get; set; }

        /// <summary>
        /// The width in pixels of the image.
        /// </summary>
        [DataMember(Name = "width")]
        public int? Width { get; set; }
    }
}
