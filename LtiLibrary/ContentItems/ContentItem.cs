using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LtiLibrary.ContentItems
{
    [DataContract]
    public class ContentItem : IFileItem, ILtiLink
    {
        public ContentItem()
        {
            Type = ContentItemType.ContentItem;
        }

        public ContentItem(ContentItemType type)
        {
            Type = type;
        }

        /// <summary>
        /// A fully qualified URL to use for the item being placed. This parameter is optional for content-items 
        /// of type application/vnd.ims.lti.v1.launch+json but is required for all other types. When not specified 
        /// for a content-item of type application/vnd.ims.lti.v1.launch+json the default launch URL used for the 
        /// same TP should be assumed.
        /// </summary>
        [DataMember(Name = "@id")]
        public string Id { get; set; }

        /// <summary>
        /// A simple name identifying the object's type. The standard context [TCP-Context] defines the following 
        /// simple names that are applicable:
        /// <para>LtiLink, FileItem</para>
        /// <para>Implementations may use a custom JSON-LD context which defines simple names for additional types 
        /// that are subtypes of ContentItem.</para>
        /// </summary>
        [DataMember(Name = "@type")]
        public ContentItemType Type { get; private set; }

        /// <summary>
        /// An object containing an @id element providing a fully qualified URL for an icon image to be placed 
        /// with the content item. A width and/or height element may also be provided. When specified the width 
        /// and height values should be a positive integer representing the number of pixels. An icon size of 
        /// 50x50 is recommended. A tool consumer may not support the display of icons; but where it does, it may 
        /// choose to use a local copy of the icon rather than linking to the URL provided (which would also allow 
        /// it to resize the image to suits its needs).
        /// </summary>
        [DataMember(Name = "icon")]
        public Image Icon { get; set; }

        /// <summary>
        /// The MIME type of the content-item.
        /// </summary>
        [Required]
        [DataMember(Name = "mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// The text to display to represent the content-item. A TP should use any text provided by the TC in the 
        /// request as the initial default, but this may be altered as part of the selection process.
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// An object containing an @id element providing a fully qualified URL for a thumbnail image to be made a 
        /// hyperlink. This allows the hyperlink to be opened within the TC from text or an image, or from both. A 
        /// width and/or height element may also be included. When specified the width and height values should be a 
        /// positive integer representing the number of pixels.
        /// </summary>
        [DataMember(Name = "thumbnail")]
        public Image Thumbnail { get; set; }

        /// <summary>
        /// The text to use as the title attribute for the content-item. When not provided, a TC may use the value 
        /// of the text parameter instead.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        #region IFileItem Parameters

        /// <summary>
        /// This parameter is an indicator to the TC as to whether it should take a copy of the 
        /// content-item and use its local copy for users to access. If true then the TP may also 
        /// indicate a time period within which the copy must be taken using the expiresAt parameter. 
        /// The default value is false.
        /// </summary>
        [DataMember(Name = "copyAdvice")]
        public bool? CopyAdvice { get; set; }

        /// <summary>
        /// The presence of this parameter indicates that the content URL is only available for a limited time 
        /// and so a copy of its contents should be stored locally if access is required for a longer period. 
        /// The parameter value should be a date/time in ISO 8601 format (e.g. 2014-03-05T12:34:56Z). This 
        /// parameter is not applicable to content-items of type application/vnd.ims.lti.v1.launch+json.
        /// </summary>
        [DataMember(Name = "expiresAt")]
        public DateTime? ExpiresAt { get; set; }

        #endregion

        #region ILtiLink Parameters

        /// <summary>
        /// A set of custom parameters to be included in the LTI launch request (as name and value pairs) 
        /// from the link being created. This parameter only applies to content-items of type 
        /// application/vnd.ims.lti.v1.launch+json.
        /// </summary>
        [DataMember(Name = "custom")]
        public IDictionary<string, string> Custom { get; set; }

        #endregion
    }
}
