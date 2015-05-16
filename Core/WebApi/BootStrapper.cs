using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using GenericToolkit.Core.EntityFramework;
using Newtonsoft.Json.Serialization;

namespace GenericToolkit.Core.WebApi
{
    public static class BootStrapper
    {
        private static readonly List<Type> _entities = new List<Type>();
        public static Action Configure = () => ConfigureWebApi();
        public static readonly IDictionary<string, Type> Controllers = new Dictionary<string, Type>();

        public static IEnumerable<Type> Entities
        {
            get { return _entities; }
        }

        public static void ConfigureWebApi()
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof (IHttpControllerSelector),
                new GenericControllerHttpControllerSelector(GlobalConfiguration.Configuration));

            dynamic json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new DefaultContractResolver();

            GlobalConfiguration.Configuration.Formatters.Remove(
                GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }

        public static void RegisterControllers(Assembly entitiesAssembly = null, Assembly dtosAssembly = null,
            string postfix = null)
        {
            if (entitiesAssembly == null) entitiesAssembly = Assembly.GetCallingAssembly();
            if (dtosAssembly == null) dtosAssembly = entitiesAssembly;
            if (postfix == null) postfix = "Dto";

            var types = entitiesAssembly.GetTypes();
            var entities = types.Where(t => t.GetInterface(typeof (IEntity).Name, false) != null && !t.Name.EndsWith(postfix)).ToArray();

            var dtoTypes = dtosAssembly.GetTypes();
            var controllerType = typeof (GenericController<,,,>);
            foreach (var entity in entities)
            {
                var entityName = entity.Name;
                var getDto = dtoTypes.SingleOrDefault(t => t.Name == string.Format("{0}Get{1}", entityName, postfix)) ??
                             entity;
                var postDto =
                    dtoTypes.SingleOrDefault(t => t.Name == string.Format("{0}Post{1}", entityName, postfix)) ?? entity;
                var putDto = dtoTypes.SingleOrDefault(t => t.Name == string.Format("{0}Put{1}", entityName, postfix)) ??
                             entity;

                var controller = controllerType.MakeGenericType(entity, getDto, postDto, putDto);

                Controllers.Add(entity.GetClassName().ToUpper(), controller);
                _entities.Add(entity);
            }

            Configure();
        }
    }
}