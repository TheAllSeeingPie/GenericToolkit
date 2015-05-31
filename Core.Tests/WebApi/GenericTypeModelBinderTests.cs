using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.Metadata.Providers;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing;
using GenericToolkit.Core.Tests.TestObjects;
using GenericToolkit.Core.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.Core.Tests.WebApi
{
    [TestClass]
    public class GenericTypeModelBinderTests
    {
        [TestMethod]
        public void Binding_a_simple_object_from_json_should_work()
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Content = new ByteArrayContent(Encoding.ASCII.GetBytes("{AProperty:\"AProperty\"}"))
            };
            var httpControllerContext = new HttpControllerContext(new HttpConfiguration(), new HttpRouteData(new HttpRoute()), httpRequestMessage);
            var actionContext = new HttpActionContext(httpControllerContext, new ReflectedHttpActionDescriptor());
            var bindingContext = new ModelBindingContext
            {
                ModelMetadata = new ModelMetadata(new EmptyModelMetadataProvider(), typeof(ITestEntity), () => new object(),typeof(ITestEntity), "AProperty")
            };
            var genericTypeModelBinder = new GenericTypeModelBinder();
            var isBound = genericTypeModelBinder.BindModel(actionContext, bindingContext);

            Assert.IsTrue(isBound);
            Assert.AreEqual("AProperty", ((ITestEntity)bindingContext.Model).AProperty);
        }
    }
}
