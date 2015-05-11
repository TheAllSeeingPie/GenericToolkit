using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;

namespace GenericToolkit.Core.WebApi
{
    public class GenericTypeModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var contentTask = actionContext.Request.Content.ReadAsStringAsync();
            contentTask.Wait();
            var content = contentTask.Result;
            var model = JsonConvert.DeserializeObject(content, TypeGenerator.GetGeneratedType(bindingContext.ModelType));
            bindingContext.Model = model;
            return true;
        }
    }
}