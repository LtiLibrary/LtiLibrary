using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LtiLibrary.NetCore.Lti.v1
{
    internal class LtiRequestJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var request = (LtiRequest) value;
            writer.WriteStartObject();
            writer.WritePropertyName("url");
            writer.WriteValue(request.Url.AbsoluteUri);
            foreach (var pair in request.Parameters.OrderBy(p => p.Key).ToList())
            {
                writer.WritePropertyName(pair.Key);
                writer.WriteValue(pair.Value);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LtiRequest);
        }

        public override bool CanRead
        {
            get { return false; }
        }
    }
}
