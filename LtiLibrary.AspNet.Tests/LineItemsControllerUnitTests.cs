using System.Net;
using LtiLibrary.Core.Outcomes.v2;
using Newtonsoft.Json;
using Xunit;

namespace LtiLibrary.AspNet.Tests
{
    public class LineItemsControllerUnitTests
    {
        [Fact]
        public void GetLineItemBeforePostReturnsNotFound()
        {
            var controller = new LineItemsController();
            ControllerSetup.RegisterContext(controller, "LineItems");
            var result = controller.GetAsync(LineItemsController.LineItemId);
            Assert.Equal(HttpStatusCode.NotFound, result.Result.StatusCode);
        }

        [Fact]
        public void PostLineItemReturnsValidLineItem()
        {
            var controller = new LineItemsController();
            ControllerSetup.RegisterContext(controller, "LineItems");
            var lineitem = new LineItem
            {
                LineItemOf = new Context {  ContextId = LineItemsController.ContextId },
                ReportingMethod = "res:Result"
            };
            var result = controller.PostAsync(LineItemsController.ContextId, lineitem);
            Assert.Equal(HttpStatusCode.Created, result.Result.StatusCode);
            var lineItem = JsonConvert.DeserializeObject<LineItem>(result.Result.Content.ReadAsStringAsync().Result);
            Assert.NotNull(lineItem);
            Assert.Equal(LineItemsController.LineItemId, lineItem.Id.ToString());
        }

        [Fact]
        public void GetLineItemsBeforePostReturnsNotFound()
        {
            var controller = new LineItemsController();
            ControllerSetup.RegisterContext(controller, "LineItems");
            var result = controller.GetAsync();
            Assert.Equal(HttpStatusCode.NotFound, result.Result.StatusCode);
        }
    }
}
