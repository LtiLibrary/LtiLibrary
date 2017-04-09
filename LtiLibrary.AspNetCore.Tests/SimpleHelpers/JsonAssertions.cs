using System.Diagnostics;
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

            public static void AssertSameObjectJson(object obj, string eventReferenceFile)
            {
                var eventJsonString = JsonConvert.SerializeObject(obj, SerializerSettings);
                var eventJObject = JObject.Parse(eventJsonString);
                var refJsonString = TestUtils.LoadReferenceJsonFile(eventReferenceFile);
                var refJObject = JObject.Parse(refJsonString);

                var diff = ObjectDiffPatch.GenerateDiff(refJObject, eventJObject);

                Debug.WriteLine(diff.NewValues);
                Debug.WriteLine(diff.OldValues);

                Assert.True(diff.NewValues == null, diff.NewValues?.ToString());
                Assert.True(diff.OldValues == null, diff.NewValues?.ToString());
            }

        public static void AssertSameJsonLdObject( JsonLdObject obj, string eventReferenceFile )
            {
               var eventJsonString = obj.ToJsonLdString();
               var eventJObject = JObject.Parse( eventJsonString );
               var refJsonString = TestUtils.LoadReferenceJsonFile( eventReferenceFile );
               var refJObject = JObject.Parse( refJsonString );

               var diff = ObjectDiffPatch.GenerateDiff( refJObject, eventJObject );

               Debug.WriteLine( diff.NewValues );
               Debug.WriteLine( diff.OldValues );

               Assert.Null( diff.NewValues );
               Assert.Null( diff.OldValues );
            }
        }
}
