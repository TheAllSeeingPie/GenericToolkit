using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace GenericToolkit.Core.WebApi
{
    //Taken from http://www.strathweb.com/2013/08/customizing-controller-discovery-in-asp-net-web-api/ and modified to suit
    public class GenericControllerHttpControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;

        public GenericControllerHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllerName = GetControllerName(request).ToUpper();

            return BootStrapper.Controllers.ContainsKey(controllerName)
                ? new HttpControllerDescriptor(_configuration, controllerName, BootStrapper.Controllers[controllerName])
                : base.SelectController(request);
        }
    }
}