using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GenericToolkit.Core.EntityFramework;

namespace GenericToolkit.Core.WebApi
{
    public static class BootStrapper
    {
        private static readonly List<Type> _entities = new List<Type>();

        public static IEnumerable<Type> Entities
        {
            get { return _entities; }
        }

        public static IDictionary<string, Type> Controllers = new Dictionary<string, Type>();

        public static void RegisterControllers(Assembly entitiesAssembly = null, Assembly dtosAssembly = null,
            string postfix = null)
        {
            if (entitiesAssembly == null)
            {
                entitiesAssembly = Assembly.GetCallingAssembly();
            }

            if (dtosAssembly == null)
            {
                dtosAssembly = entitiesAssembly;
            }

            if (postfix == null)
            {
                postfix = "Dto";
            }

            var types = entitiesAssembly.GetTypes();
            var entities =
                types.Where(t => t.GetInterface(typeof (IEntity).Name, false) != null && !t.Name.EndsWith(postfix))
                    .ToArray();

            var dtoTypes = dtosAssembly.GetTypes();
            var controllerType = typeof (GenericController<,,,>);
            foreach (var entity in entities)
            {
                var entityName = entity.Name;
                var getDto = dtoTypes.Single(t => t.Name == string.Format("{0}Get{1}", entityName, postfix));
                var postDto = dtoTypes.Single(t => t.Name == string.Format("{0}Post{1}", entityName, postfix));
                var putDto = dtoTypes.Single(t => t.Name == string.Format("{0}Put{1}", entityName, postfix));

                var controller = controllerType.MakeGenericType(entity, getDto, postDto, putDto);
                
                Controllers.Add(entity.GetClassName().ToUpper(), controller);
                _entities.Add(entity);
            }
        }
    }
}