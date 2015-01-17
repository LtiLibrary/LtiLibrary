using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace LtiLibrary.Core.Common
{
    public class JsonLdObjectContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);
            var orderedList = new List<JsonProperty>();

            var context = list.SingleOrDefault(p => p.PropertyName.Equals("@context"));
            if (context != null) orderedList.Add(context);

            var objectType = list.SingleOrDefault(p => p.PropertyName.Equals("@type"));
            if (objectType != null) orderedList.Add(objectType);

            var id = list.SingleOrDefault(p => p.PropertyName.Equals("@id"));
            if (id != null) orderedList.Add(id);

            orderedList.AddRange(
                list.Where(jsonProperty => 
                    !jsonProperty.PropertyName.Equals("@context") && 
                    !jsonProperty.PropertyName.Equals("@type") && 
                    !jsonProperty.PropertyName.Equals("@id")));

            return orderedList;
        }
    }
}
