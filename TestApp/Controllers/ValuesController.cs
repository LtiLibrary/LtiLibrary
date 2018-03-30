using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Clients;
using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Mvc;

namespace TestApp.Controllers
{
    [Route("api/[controller]")]
    public class LtiController : Controller
    {
        string key = "jisc.ac.uk";
        string secret = "secret";
        static double callCount = 0.0;
        //GET api/values
        [HttpPost]
        public async Task<IEnumerable<string>> Launch()
        {
            callCount++;

            LtiRequest ltiReqest = await Request.ParseLtiRequestAsync();
            var client = new HttpClient();
            var outcomeUrl = ltiReqest.LisOutcomeServiceUrl;
            string lisResultSourcedId = ltiReqest.LisResultSourcedId;
            await Outcomes1Client.ReplaceResultAsync(client, outcomeUrl, key, secret, lisResultSourcedId, null,   resultLtiLaunchUrl: "https://f51f4fa4.ngrok.io/api/review2/");

            return new string[] { "value1", "value2" };
        }
    }

    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        string key = "jisc.ac.uk";
        string secret = "secret";
        static double callCount = 0.0;
        //GET api/values
        [HttpPost]
        public async Task<IEnumerable<string>> Review()
        {
           

            return new string[] { "review mode"};
        }
    }
    [Route("api/[controller]")]
    public class Review2Controller : Controller
    {
        string key = "jisc.ac.uk";
        string secret = "secret";
        static double callCount = 0.0;
        //GET api/values
        [HttpPost]
        public async Task<IEnumerable<string>> Review()
        {


            return new string[] { "review mode 2" };
        }
    }
}
