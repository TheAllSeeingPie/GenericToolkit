using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher.Fakes;
using GenericToolkit.Core.WebApi;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.Core.Tests.WebApi
{
    [TestClass]
    public class GenericControllerHttpControllerSelectorTests
    {
        [TestMethod]
        public void Selector_should_select_a_configured_controller()
        {
            using (ShimsContext.Create())
            {
                ShimDefaultHttpControllerSelector.AllInstances.GetControllerNameHttpRequestMessage = (controllerSelector, message) => "TEST";

                BootStrapper.Controllers.Add("TEST", typeof (GenericController<,,,>));
                var selector = new GenericControllerHttpControllerSelector(new HttpConfiguration());
                var controller = selector.SelectController(new HttpRequestMessage());

                Assert.AreEqual(typeof (GenericController<,,,>), controller.ControllerType);
            }
        }
    }
}
