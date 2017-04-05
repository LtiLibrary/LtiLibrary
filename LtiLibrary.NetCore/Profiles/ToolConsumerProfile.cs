using System.Collections.Generic;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti2;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Profiles
{
    /// <summary>
    /// The ToolConsumerProfile advertises the Tool Consumer's capabilities and services offered.
    /// </summary>
    /// <remarks>
    /// The ToolConsumerProfile (and all of its members) conform to JSON-LD conventions. Briefly, JSON-LD adds 
    /// semantics to a JSON document by associating properties that may appear in a document with well-defined 
    /// data types through the use of a so-called "context".
    /// 
    /// The JSON-LD standard reserves a handful of property names and tokens that have special meaning. These 
    /// names and tokens, described below, begin with the '@' symbol.
    /// 
    /// @context - Used to reference (by URI or by value) a context which declares the simple names that appear 
    /// throughout a JSON document.
    /// 
    /// @id - Used to uniquely identify things that are being described in the JSON document. The value of an 
    /// @id property is either a fully-qualified URI, a CURIE, or a simple name that expands to a fully-qualified 
    /// URI by virtue of the rules defined in the JSON-LD Context.
    /// 
    /// @type - Used to set the data type of an object or property value.
    /// </remarks>
    public class ToolConsumerProfile : JsonLdObject
    {
        public ToolConsumerProfile()
        {
            ExternalContextId = LtiConstants.ToolConsumerProfileContextId;
            Type = LtiConstants.ToolConsumerProfileType;
        }

        /// <summary>
        /// A capability offered by the Tool Consumer to its integration partners.
        /// </summary>
        [JsonProperty("capability_offered")]
        public IEnumerable<string> CapabilityOffered { get; set; }
        
        /// <summary>
        /// A globally unique identifier for the service provider. As a best practice, this value should match an 
        /// Internet domain name assigned by ICANN, but any globally unique identifier is acceptable. 
        /// </summary>
        [JsonProperty("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// The identifier for an LTI version that the version supports. A given product (Tool or Tool Consumer) 
        /// may support multiple versions, but only one version is selected for use in the integration contract. 
        /// Should match lti_version launch parameter.
        /// </summary>
        [JsonProperty("lti_version")]
        public string LtiVersion { get; set; }

        /// <summary>
        /// An inverse attribute which references the ProductInstance within which this ToolConsumerProfile is defined.
        /// </summary>
        [JsonProperty("product_instance")]
        public ProductInstance ProductInstance { get; set; }

        /// <summary>
        /// The descriptor for a service offered by the product (Tool or Tool Consumer).
        /// </summary>
        [JsonProperty("service_offered")]
        public IEnumerable<RestService> ServiceOffered { get; set; }
    }
}
