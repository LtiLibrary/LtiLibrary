using LtiLibrary.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LtiLibrary.ContentItems
{
    [DataContract]
    public class ContentItemPlacementResponse
    {
        public ContentItemPlacementResponse()
        {
            Context = LtiConstants.ContentItemPlacementContext;
        }

        /// <summary>
        /// For most implementations, the value will be the single URI for the standard context associated with 
        /// the application/vnd.ims.lti.v1.contentitemplacement+json media type. In this case, the value will be
        /// <para>"http://purl.imsglobal.org/ctx/lti/v1/ContentItemPlacement"</para>
        /// </summary>
        [DataMember(Name = "@context")]
        public string Context { get; private set; }

        [DataMember(Name = "@graph")]
        public IEnumerable<ContentItemPlacement> Graph { get; set; }
    }
}
