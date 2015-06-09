using System;
using Newtonsoft.Json.Serialization;

namespace LtiLibrary.Core.Common
{
    public class JsonLdObjectContractResolver : DefaultContractResolver
    {
        public override JsonContract ResolveContract(Type type)
        {
            if (typeof(JsonLdObject).IsAssignableFrom(type))
            {
                var contract = base.ResolveContract(type);
                contract.Converter = null;
                return contract;
            }
            return base.ResolveContract(type);
        }

    }
}
