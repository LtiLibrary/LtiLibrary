using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiLibrary.NetCore.Lis.v1
{
    internal class JsonLdContextRoleConverter : StringEnumConverter
    {
        private const string UrnPrefix = "urn:lti:role:ims/lis/";

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            if (!objectType.GetTypeInfo().IsAssignableFrom(typeof(ContextRole)))
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
                    if (TryExtractRole(value, out ContextRole contextRole))
                    {
                        return contextRole;
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

            if (value.GetType().GetTypeInfo().IsAssignableFrom(typeof(ContextRole)))
            {
                var e = (Enum)value;
                var enumName = e.ToString("G");
                writer.WriteValue($"lism:{enumName}");
                return;
            }
            base.WriteJson(writer, value, serializer);
        }

        private bool TryExtractRole(string source, out ContextRole role)
        {
            var roleFound = true;
            role = default(ContextRole);

            do
            {
                // Parse cases like lism:<role>
                int start = source.IndexOf(':') + 1;
                if (Enum.TryParse(source.Substring(start), out role))
                    break;

                // Parse cases like urn: urn:lti:role:ims/lis/<role>/<sub-role>
                start = source.IndexOf(UrnPrefix) + UrnPrefix.Length;
                var end = source.IndexOf('/', start);
                if (Enum.TryParse(source.Substring(start, end - start), out role))
                    break;

                // Parse cases like uri: http://purl.imsglobal.org/vocab/lis/v2/mm#<role>
                start = source.LastIndexOf('#') + 1;
                if (Enum.TryParse(source.Substring(start), out role))
                    break;

                roleFound = false;
            } while (!roleFound);

            return roleFound;
        }
    }
}
