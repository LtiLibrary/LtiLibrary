using System;
using System.Net;
using LtiLibrary.Core.Outcomes.v2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace LtiLibrary.AspNet.Tests
{
    [TestClass]
    public class LineItemsControllerUnitTests
    {
        [TestMethod]
        public void GetLineItemBeforePostReturnsNotFound()
        {
            var controller = new LineItemsController();
            ControllerSetup.RegisterContext(controller, "LineItems");
            var result = controller.Get(LineItemsController.LineItemId);
            Assert.AreEqual(HttpStatusCode.NotFound, result.Result.StatusCode);
        }

        [TestMethod]
        public void PostLineItemReturnsValidLineItem()
        {
            var controller = new LineItemsController();
            ControllerSetup.RegisterContext(controller, "LineItems");
            var lineitem = new LineItem
            {
                LineItemOf = new Context {  ContextId = LineItemsController.ContextId },
                ReportingMethod = new Uri("res:Result")
            };
            var result = controller.Post(lineitem);
            Assert.AreEqual(HttpStatusCode.Created, result.Result.StatusCode);
            var lineItem = JsonConvert.DeserializeObject<LineItem>(result.Result.Content.ReadAsStringAsync().Result);
            Assert.IsNotNull(lineItem);
            Assert.AreEqual(LineItemsController.LineItemId, lineItem.Id);
        }

        [TestMethod]
        public void GetLineItemsBeforePostReturnsNotFound()
        {
            var controller = new LineItemsController();
            ControllerSetup.RegisterContext(controller, "LineItems");
            var result = controller.Get();
            Assert.AreEqual(HttpStatusCode.NotFound, result.Result.StatusCode);
        }
    }
}
