using System.Xml.Serialization;

namespace LtiLibrary.NetCore.Lti.v1
{
    /// <summary>
    /// Add Canvas resultData element to ResultType.
    /// </summary>
    public partial class ResultType 
    {
        /// <summary>
        /// Get or set optional resultData element supported by Canvas.
        /// </summary>
        [XmlElement(ElementName = "resultData")]
        public DataType ResultData { get; set; }
    }
}
