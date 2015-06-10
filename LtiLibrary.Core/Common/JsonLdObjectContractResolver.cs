using System;
using Newtonsoft.Json.Serialization;

namespace LtiLibrary.Core.Common
{
    /// <summary>
    /// DefaultContractResolver with null Converter for JsonLdObjects
    /// to prevent recusive calls to JsonLdObjectConverter.
    /// </summary>
    public class JsonLdObjectContractResolver : DefaultContractResolver
    {
        public override JsonContract ResolveContract(Type type)
        {
            if (typeof(JsonLdObject).IsAssignableFrom(type))
            {
                var contract = base.ResolveContract(type);
                
                // Set the Convert to null to prevent recursive call to JsonLdObjectConverter
                contract.Converter = null;

                return contract;
            }
            return base.ResolveContract(type);
        }

    }
}
