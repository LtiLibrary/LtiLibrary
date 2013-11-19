using System;
using System.Linq;
using LtiLibrary.Models;
using Provider.Models;

namespace Provider.Extensions
{
    public static class LtiInboundRequestExtensions
    {
        public static string GetDistrictName(this LtiInboundRequest request)
        {
            // consumer_district_id - NCES id for US school district
            var districtId = request.CustomParameters["custom_district_id"];
            if (string.IsNullOrWhiteSpace(districtId)) return default(string);

            using (var db = new ProviderContext())
            {
                District district;
                if (districtId.StartsWith("nces.ed.gov:", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = districtId.Split(':')[1];
                    district = db.Districts.SingleOrDefault(d => d.DistrictId == id);
                }
                else
                {
                    district = db.Districts.SingleOrDefault(d => d.StateDistrictId == districtId);
                }
                return district == null ? default(string) : district.Name;
            }
        }

        public static string GetSchoolName(this LtiInboundRequest request)
        {
            // custom_school_id - NCES id for US school
            var schoolId = request.CustomParameters["custom_school_id"];
            if (string.IsNullOrWhiteSpace(schoolId)) return default(string);

            using (var db = new ProviderContext())
            {
                School school;
                if (schoolId.StartsWith("nces.ed.gov:", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = schoolId.Split(':')[1];
                    school = db.Schools.SingleOrDefault(s => s.SchoolId == id);
                }
                else
                {
                    school = db.Schools.SingleOrDefault(s => s.DistrictSchoolId == schoolId);
                }
                return school == null ? default(string) : school.Name;
            }

        }

        public static string GetStateName(this LtiInboundRequest request)
        {
            // custom_state_id - US State or Canadian Provider ID (e.g. AL)
            var stateId = request.CustomParameters["custom_state_id"];
            if (string.IsNullOrWhiteSpace(stateId)) return default(string);

            using (var db = new ProviderContext())
            {
                var state = db.States.SingleOrDefault(s => s.StateId == stateId);
                return state == null ? default(string) : state.Name;
            }
        }

        public static string GetUserName(this LtiInboundRequest request)
        {
            return request.CustomParameters["custom_username"] ?? request.UserId;
        }
    }
}