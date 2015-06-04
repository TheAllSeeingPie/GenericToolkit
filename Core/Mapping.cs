using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GenericToolkit.Core
{
    public static class Mapping
    {
        //Attempt scanning the entire AppDomain - Not recommended but sometimes a necessary evil
        //Doing this in a static class and only once isn't going to be the worst thing you could do in C#
        private static readonly Type[] ValueResolvers = TypeScanner.All
            .Where(t => t.BaseType != null && t.BaseType.GetInterfaces().Any(i => i == typeof (IValueResolver)))
            .Where(t => t.BaseType.GenericTypeArguments.Any() && t.BaseType.GenericTypeArguments.Count() == 2)
            .ToArray();

        public static void Register(IEnumerable<Type> entities, IEnumerable<Type> dtos, Func<Type, Type, bool> namingConvention = null)
        {
            if (namingConvention == null)
            {
                namingConvention = (type, type1) => type1.Name.StartsWith(type.Name);
            }

            var dtoArray = dtos as Type[] ?? dtos.ToArray();
            foreach (var e in entities)
            {
                var entity = e;
                var entityProperties = entity.GetProperties();
                
                foreach (var dto in dtoArray.Where(dto => namingConvention(entity, dto)))
                {
                    var toMap = Mapper.CreateMap(entity, dto);
                    var fromMap = Mapper.CreateMap(dto, entity);

                    foreach (var prop in dto.GetProperties())
                    {
                        var dtoProp = prop;
                        var entityProp = entityProperties.SingleOrDefault(p => dtoProp.Name.StartsWith(p.Name));

                        if (entityProp == null)
                        {
                            continue;
                        }

                        var toResolver = ValueResolvers.FirstOrDefault(vr =>
                            vr.BaseType.GenericTypeArguments.First() == entityProp.PropertyType &&
                            vr.BaseType.GenericTypeArguments.Last() == dtoProp.PropertyType);

                        if (toResolver != null)
                        {
                            toMap.ForMember(dtoProp.Name,
                                member => member.ResolveUsing(toResolver).FromMember(entityProp.Name));
                        }

                        var fromResolver = ValueResolvers.FirstOrDefault(vr =>
                            vr.BaseType.GenericTypeArguments.First() == dtoProp.PropertyType &&
                            vr.BaseType.GenericTypeArguments.Last() == entityProp.PropertyType);

                        if (fromResolver != null)
                        {
                            fromMap.ForMember(entityProp.Name,
                                member => member.ResolveUsing(fromResolver).FromMember(dtoProp.Name));
                        }
                    }
                }
            }
        }
    }
}
