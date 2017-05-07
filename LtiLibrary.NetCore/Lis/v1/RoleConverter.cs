using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiLibrary.NetCore.Lis.v1
{
    internal class RoleConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            if (!objectType.GetTypeInfo().IsAssignableFrom(typeof(Role)))
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            if (reader.TokenType != JsonToken.String)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            var value = reader.Value.ToString();
            var index = value.IndexOf(':');

            if (index > -1)
            {
                if (value.Length > index)
                {
                    Role enumValue;
                    if (Enum.TryParse(value.Substring(5), out enumValue))
                    {
                        return enumValue;
                    }
                    throw new JsonSerializationException($"Cannot convert {value} to {objectType.Name}.");
                }
                throw new JsonSerializationException($"Cannot convert {value} to {objectType.Name}.");
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                base.WriteJson(writer, null, serializer);
                return;
            }

            if (value.GetType().GetTypeInfo().IsAssignableFrom(typeof(Role)))
            {
                var e = (Enum)value;
                var enumName = e.ToString("G");
                writer.WriteValue($"lism:{enumName}");
                return;
            }
            base.WriteJson(writer, value, serializer);
        }
    }
}
