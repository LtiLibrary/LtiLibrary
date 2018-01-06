using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.SimpleHelpers
{
    internal static class JsonAssertions
    {
        private static readonly JsonSerializerSettings SerializerSettings;

        static JsonAssertions()
        {
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        }

        public static void AssertSameObjectJson(object actualObject, string expectedJsonFile)
        {
            var actualJsonString = JsonConvert.SerializeObject(actualObject, SerializerSettings);
            var actualJObject = JObject.Parse(actualJsonString);
            var expectedJsonString = TestUtils.LoadReferenceJsonFile(expectedJsonFile);
            var expectedJObject = JObject.Parse(expectedJsonString);

            AssertSameObjectJson(expectedJObject, actualJObject);
        }

        public static void AssertSameJsonLdObject(JsonLdObject actualJsonLdObject, string expectedJsonFile)
        {
            var actualJsonString = actualJsonLdObject.ToJsonLdString();
            var actualJObject = JObject.Parse(actualJsonString);
            var expectedJsonString = TestUtils.LoadReferenceJsonFile(expectedJsonFile);
            var expectedJObject = JObject.Parse(expectedJsonString);

            AssertSameObjectJson(expectedJObject, actualJObject);
        }

        internal static void AssertSameObjectJson(JObject expected, JObject actual)
        {
            var diff = ObjectDiffPatch.GenerateDiff(expected, actual);
            Assert.True(diff.NewValues == null && diff.OldValues == null, "Expected:\n" + diff.OldValues + "\nActual:\n" + diff.NewValues);
        }
    }
}
