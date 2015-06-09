using System.Diagnostics;
using LtiLibrary.Core.Tests.TestHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Xunit;

namespace LtiLibrary.Core.Tests.SimpleHelpers
{
        internal static class JsonAssertions
        {
            private static readonly JsonSerializerSettings SerializerSettings;

            static JsonAssertions()
            {
                SerializerSettings = new JsonSerializerSettings();
                SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            }

            public static void AssertSameObjectJson(object obj, string eventReferenceFile)
            {

                var eventJsonString = JsonConvert.SerializeObject(obj, SerializerSettings);
                var eventJObject = JObject.Parse(eventJsonString);
                var refJsonString = TestUtils.LoadReferenceJsonFile(eventReferenceFile);
                var refJObject = JObject.Parse(refJsonString);

                var diff = ObjectDiffPatch.GenerateDiff(refJObject, eventJObject);

                Trace.WriteLine(diff.NewValues);
                Trace.WriteLine(diff.OldValues);

                Assert.Null(diff.NewValues);
                Assert.Null(diff.OldValues);
            }
        }
}
