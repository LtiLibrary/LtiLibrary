using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Consumer.Models;

namespace Consumer.Controllers
{
    public class LocationController : Controller
    {
        private ConsumerContext db = new ConsumerContext();

        public ActionResult GetDistricts(string stateId)
        {
            dynamic districts = null;

            if (!string.IsNullOrEmpty(stateId))
            {
                districts = db.Districts.Where(d => d.StateId.Equals(stateId))
                    .Select(item => new { Id = item.DistrictId, Name = item.Name })
                    .OrderBy(item => item.Name);
            }

            return Json(districts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchools(string districtId)
        {
            dynamic schools = null;

            if (!string.IsNullOrEmpty(districtId))
            {
                schools = db.Schools.Where(s => s.DistrictId.Equals(districtId))
                    .Select(item => new { Id = item.SchoolId, Name = item.Name })
                    .OrderBy(item => item.Name)
                    .ToList();
            }

            return Json(schools, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetStates(string term)
        //{
        //    dynamic states;

        //    if (string.IsNullOrEmpty(term))
        //    {
        //        states = db.States.Select(item => new { Id = item.StateId, Name = item.Name })
        //            .OrderBy(item => item.Name);
        //    }
        //    else
        //    {
        //        states = db.States.Where(s => s.StateId.Equals(term) || s.Name.StartsWith(term))
        //            .Select(item => new { Id = item.StateId, Name = item.Name })
        //            .OrderBy(item => item.Name);
        //    }
        //    return Json(states, JsonRequestBehavior.AllowGet);
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}