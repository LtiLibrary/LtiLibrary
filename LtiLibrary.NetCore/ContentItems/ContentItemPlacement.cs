using LtiLibrary.NetCore.Lti1;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.ContentItems
{
    /// <summary>
    /// Represents a ContentItemPlacement.
    /// </summary>
    public class ContentItemPlacement
    {
        /// <summary>
        /// A valid value for the height attribute of the HTML element which will be created by the TC to refer to 
        /// the content-item. This may also be used for sizing a popup window when this document target is requested. 
        /// Typically this is a positive integer value representing the number of pixels.
        /// </summary>
        [JsonProperty("displayHeight")]
        public int? DisplayHeight { get; set; }

        /// <summary>
        /// A valid value for the width attribute of the HTML element which will be created by the TC to refer to the 
        /// content-item. This may also be used for sizing a popup window when this document target is requested. 
        /// Typically this is a positive integer value representing the number of pixels.
        /// </summary>
        [JsonProperty("displayWidth")]
        public int? DisplayWidth { get; set; }

        /// <summary>
        /// This parameter is used to determine where the content-item being added should be opened. It should be one 
        /// of the values included in the accept_presentation_document_targets request parameter (see above); if the 
        /// parameter was not included in the request then this parameter should be omitted from the response. When 
        /// omitted, the TC should use a plain anchor tag; the target parameter (see below) may be used to set where 
        /// the content-item is opened.
        /// </summary>
        [JsonProperty("presentationDocumentTarget")]
        public DocumentTarget? PresentationDocumentTarget { get; set; }

        /// <summary>
        /// The windowTarget parameter to be used for any hyperlink used to open the content-item may be specified 
        /// using this parameter. Note that this parameter is distinct from the presentation_document_target parameter 
        /// (see above) which is used to determine how the content-item is opened. This parameter is most useful when 
        /// a presentation_document_target of window is specified. 
        /// </summary>
        [JsonProperty("windowTarget")]
        public string WindowTarget { get; set; }
    }
}
