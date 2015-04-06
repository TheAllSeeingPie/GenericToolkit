using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace GenericToolkit.Core.WebApi
{
    //Taken from http://www.strathweb.com/2013/08/customizing-controller-discovery-in-asp-net-web-api/ and modified to suit
    public class BypassCacheSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;

        public BypassCacheSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.FullName == "GenericControllers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            var types = assembly.GetTypes(); //GetExportedTypes doesn't work with dynamic assemblies
            var matchedTypes = types.Where(i => typeof(IHttpController).IsAssignableFrom(i)).ToList();

            var controllerName = base.GetControllerName(request);
            var matchedController =
                matchedTypes.FirstOrDefault(i => i.Name.ToLower() == controllerName.ToLower() + "controller");

            return new HttpControllerDescriptor(_configuration, controllerName, matchedController);
        }
    }
}