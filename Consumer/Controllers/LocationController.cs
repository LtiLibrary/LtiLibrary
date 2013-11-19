using System.Linq;
using System.Web.Mvc;
using Consumer.Models;

namespace Consumer.Controllers
{
    public class LocationController : Controller
    {
        private readonly ConsumerContext _db = new ConsumerContext();

        public ActionResult GetDistricts(string stateId)
        {
            dynamic districts = null;

            if (!string.IsNullOrEmpty(stateId))
            {
                districts = _db.Districts.Where(d => d.StateId.Equals(stateId))
                    .Select(item => new { Id = item.DistrictId, item.Name })
                    .OrderBy(item => item.Name);
            }

            return Json(districts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchools(string districtId)
        {
            dynamic schools = null;

            if (!string.IsNullOrEmpty(districtId))
            {
                schools = _db.Schools.Where(s => s.DistrictId.Equals(districtId))
                    .Select(item => new { Id = item.SchoolId, item.Name })
                    .OrderBy(item => item.Name)
                    .ToList();
            }

            return Json(schools, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}