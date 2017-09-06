﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace LtiLibrary.NetCore.Common
{
    /// <summary>
    /// Serialize a JsonLdObject to JSON representation.
    /// </summary>
    /// <remarks>
    /// Collects JsonLdObject Terms and writes them first as @context.
    /// Does not convert JSON to a JsonLdObject.
    /// </remarks>
    internal class JsonLdObjectConverter : JsonConverter
    {
        private static readonly Type JsonLdObjectType = typeof(IJsonLdObject);
        private static readonly Type JsonLdObjectArrayType = typeof (IEnumerable<IJsonLdObject>);

        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The Newtonsoft.Json.JsonWriter to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Collect the external contexts. Note that the last one wins.
            // See http://www.w3.org/TR/json-ld/#advanced-context-usage
            Uri externalContextId = null;
            if (JsonLdObjectType.GetTypeInfo().IsInstanceOfType(value))
            {
                var obj = (IJsonLdObject) value;
                externalContextId = GetExternalContextId(obj);
            }

            // Collect the local terms
            IDictionary<string, object> terms = null;
            if (JsonLdObjectType.GetTypeInfo().IsInstanceOfType(value))
            {
                var obj = (IJsonLdObject) value;
                terms = GetTerms(obj);
            }

            // Use a new contract resolver to parse the object so that this
            // version of WriteJson is not called recursively
            serializer.ContractResolver = new JsonLdObjectContractResolver(true);
            serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            var o = JObject.FromObject(value, serializer);

            // Add the @context and write the JSON representation.
            // If there is only an external context, add a simple JProperty.
            if (externalContextId != null && (terms == null || terms.Count == 0))
            {
                if (!o.HasValues || o.First.Type != JTokenType.Property || !((JProperty) o.First).Name.Equals("@context"))
                {
                    o.AddFirst(new JProperty("@context", externalContextId));
                }
                o["@context"] = externalContextId;
            }
            // If there are only terms, add a JObject with each term as a JProperty
            else if (externalContextId == null && terms != null && terms.Count > 0)
            {
                var termsObject = new JObject();
                foreach (var key in terms.Keys)
                {
                    termsObject.Add(new JProperty(key, terms[key]));
                }
                o.Remove("@context");
                o.AddFirst(new JProperty("@context", termsObject));
            }
            // If there is both an external context and internal contexts (terms),
            // then add a JArray (http://www.w3.org/TR/json-ld/#advanced-context-usage)
            else if (externalContextId != null && terms.Count > 0)
            {
                var termsObject = new JObject();
                foreach (var key in terms.Keys)
                {
                    termsObject.Add(new JProperty(key, terms[key]));
                }
                o.Remove("@context");
                o.AddFirst(new JProperty("@context", new JArray
                {
                    externalContextId,
                    termsObject
                }));
            }
            o.WriteTo(writer);
        }

        /// <summary>
        /// Get the external context from this object.
        /// </summary>
        /// <param name="obj">The JsonLdObject.</param>
        /// <returns>The last external context.</returns>
        private static Uri GetExternalContextId(IJsonLdObject obj)
        {
            if (obj == null) return null;

            var externalContextId = obj.ExternalContextId;

            // Walk the object to find embedded JsonLdObjects
            foreach (var propertyInfo in obj.GetType().GetTypeInfo().GetProperties())
            {
                if (!propertyInfo.CanRead) continue;

                if (JsonLdObjectType.GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var propertyValue = (IJsonLdObject)propertyInfo.GetValue(obj, null);
                    var contextId = GetExternalContextId(propertyValue);
                    if (contextId != null)
                        externalContextId = contextId;
                }
                else if (JsonLdObjectArrayType.GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var propertyValue = (IEnumerable<IJsonLdObject>)propertyInfo.GetValue(obj, null);
                    if (propertyValue != null)
                    {
                        foreach (var jsonLdObject in propertyValue)
                        {
                            var contextId = GetExternalContextId(jsonLdObject);
                            if (contextId != null)
                                externalContextId = contextId;
                        }
                    }
                }
            }
            return externalContextId;
        }

        /// <summary>
        /// Collect the terms from the JsonLdObject and all its child properties.
        /// </summary>
        /// <param name="obj">The JsonLdObject.</param>
        /// <returns>The list of terms.</returns>
        private static IDictionary<string, object> GetTerms(IJsonLdObject obj)
        {
            var terms = new Dictionary<string, object>();
            if (obj?.Terms == null) return terms;

            foreach (var key in obj.Terms.Keys)
            {
                terms[key] = obj.Terms[key];
            }

            // Check for terms already in the @context
            var context = obj.Context as JArray;
            if (context != null && context.Count > 1)
            {
                for (int index = 1; index < context.Count; index++)
                {
                    var contextObject = context[index] as JObject;
                    var term = contextObject?.First as JProperty;
                    if (term != null)
                    {
                        terms[term.Name] = term.Value;
                    }
                }
            }

            // Walk the object to find embedded JsonLdObjects
            foreach (var propertyInfo in obj.GetType().GetTypeInfo().GetProperties())
            {
                if (!propertyInfo.CanRead) continue;

                if (JsonLdObjectType.GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var propertyValue = (IJsonLdObject) propertyInfo.GetValue(obj, null);
                    var propertyTerms = GetTerms(propertyValue);
                    foreach (var key in propertyTerms.Keys)
                    {
                        terms[key] = propertyTerms[key];
                    }
                }
                else if (JsonLdObjectArrayType.GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var propertyValue = (IEnumerable<IJsonLdObject>) propertyInfo.GetValue(obj, null);
                    if (propertyValue != null)
                    {
                        foreach (var jsonLdObject in propertyValue)
                        {
                            var propertyTerms = GetTerms(jsonLdObject);
                            foreach (var key in propertyTerms.Keys)
                            {
                                terms[key] = propertyTerms[key];
                            }
                        }
                    }
                }
            }
            return terms;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (JsonLdObject);
        }
    }
}
