using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using GenericToolkit.Core.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.Core.Tests.WebApi
{
    [TestClass]
    public class GenericControllerHttpControllerSelectorTests
    {
        [TestMethod]
        public void Selector_should_select_a_configured_controller()
        {
            BootStrapper.Controllers.Add("TEST", typeof (GenericController<,,,>));
            
            var selector = new GenericControllerHttpControllerSelector(new HttpConfiguration());
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.SetRouteData(new HttpRouteData(new HttpRoute("default"), new HttpRouteValueDictionary(new Dictionary<string, object> {{"controller", "TEST"}})));
            var controller = selector.SelectController(httpRequestMessage);

            Assert.AreEqual(typeof (GenericController<,,,>), controller.ControllerType);
        }
    }
}