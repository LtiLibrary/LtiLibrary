using System;
using System.Xml.Serialization;

namespace LtiLibrary.NetCore.Lti.v1
{
    /// <summary>
    /// resultData data type supported by Canvas.
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "Data.Type", Namespace="http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0")]
    [XmlRoot("resultData", Namespace="http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0", IsNullable = true)]
    public class DataType
    {
        /// <summary>
        /// Get or set optional submission detail text. Can contain HTML. Supported by Canvas.
        /// </summary>
        [XmlElement(ElementName = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Get or set optional submission detail URL. Supported by Canvas.
        /// </summary>
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Get or set optional submission detail LTI Launch URL. Supported by Canvas.
        /// </summary>
        [XmlElement(ElementName = "ltiLaunchUrl")]
        public string LtiLaunchUrl { get; set; }
    }
}
