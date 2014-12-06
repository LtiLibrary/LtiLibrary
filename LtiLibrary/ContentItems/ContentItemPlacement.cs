using System.Runtime.Serialization;
using LtiLibrary.Lti1;

namespace LtiLibrary.ContentItems
{
    [DataContract]
    public class ContentItemPlacement
    {
        public ContentItemPlacement()
        {
            Type = "ContentItemPlacement";
        }

        /// <summary>
        /// A simple name identifying the object's type. The standard context [TCP-Context] defines the following 
        /// simple names that are applicable:
        /// <para>ToolConsumerProfile</para>
        /// <para>Implementations may use a custom JSON-LD context which defines simple names for additional types 
        /// that are subtypes of ContentItemPlacement.</para>
        /// </summary>
        [DataMember(Name = "@type")]
        public string Type { get; private set; }

        /// <summary>
        /// A valid value for the height attribute of the HTML element which will be created by the TC to refer to 
        /// the content-item. This may also be used for sizing a popup window when this document target is requested. 
        /// Typically this is a positive integer value representing the number of pixels.
        /// </summary>
        [DataMember(Name = "displayHeight")]
        public int? DisplayHeight { get; set; }

        /// <summary>
        /// A valid value for the width attribute of the HTML element which will be created by the TC to refer to the 
        /// content-item. This may also be used for sizing a popup window when this document target is requested. 
        /// Typically this is a positive integer value representing the number of pixels.
        /// </summary>
        [DataMember(Name = "displayWidth")]
        public int? DisplayWidth { get; set; }

        /// <summary>
        /// The content-item to be placed.
        /// </summary>
        [DataMember(Name = "placementOf")]
        public ContentItem PlacementOf { get; set; }

        /// <summary>
        /// This parameter is used to determine where the content-item being added should be opened. It should be one 
        /// of the values included in the accept_presentation_document_targets request parameter (see above); if the 
        /// parameter was not included in the request then this parameter should be omitted from the response. When 
        /// omitted, the TC should use a plain anchor tag; the target parameter (see below) may be used to set where 
        /// the content-item is opened.
        /// </summary>
        [DataMember(Name = "presentation_document_target")]
        public DocumentTarget? PresentationDocumentTarget { get; set; }

        /// <summary>
        /// The windowTarget parameter to be used for any hyperlink used to open the content-item may be specified 
        /// using this parameter. Note that this parameter is distinct from the presentation_document_target parameter 
        /// (see above) which is used to determine how the content-item is opened. This parameter is most useful when 
        /// a presentation_document_target of window is specified. 
        /// </summary>
        [DataMember(Name = "windowTarget")]
        public string WindowTarget { get; set; }
    }
}
