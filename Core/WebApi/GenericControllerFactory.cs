using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using GenericToolkit.Core.EntityFramework;

namespace GenericToolkit.Core.WebApi
{
    public static class GenericControllerFactory
    {
        private static readonly ConcurrentDictionary<string, Type> GeneratedControllerTypes = new ConcurrentDictionary<string, Type>();

        public static GenericController<TEntity, TGet, TPost, TPut> Generate<TEntity, TGet, TPost, TPut>(
            GenericContext context)
            where TEntity : class, IEntity
            where TGet : class
            where TPost : class
            where TPut : class, IEntity
        {
            var name = string.Format("{0}Controller", typeof(TEntity).Name.Substring(1));
            Type type;
            if (!GeneratedControllerTypes.TryGetValue(name, out type))
            {
                var parent = typeof (GenericController<TEntity, TGet, TPost, TPut>);
                var typeBuilder = ModuleBuilder.DefineType(name, TypeAttributes.Public
                                                                 | TypeAttributes.Class
                                                                 | TypeAttributes.AutoClass
                                                                 | TypeAttributes.AnsiClass
                                                                 | TypeAttributes.Serializable
                                                                 | TypeAttributes.BeforeFieldInit,
                    parent);

                var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                    CallingConventions.Standard, Type.EmptyTypes);

                var parentCtor = parent.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null,
                    Type.EmptyTypes, null);

                var constructorIL = constructorBuilder.GetILGenerator();
                constructorIL.Emit(OpCodes.Ldarg_0);
                constructorIL.Emit(OpCodes.Call, parentCtor);
                constructorIL.Emit(OpCodes.Ret);

                constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                    CallingConventions.Standard, new[] {typeof (GenericContext)});

                parentCtor = parent.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null,
                    new[] {typeof (GenericContext)}, null);

                constructorIL = constructorBuilder.GetILGenerator();
                constructorIL.Emit(OpCodes.Ldarg_0);
                constructorIL.Emit(OpCodes.Ldarg_1);
                constructorIL.Emit(OpCodes.Call, parentCtor);
                constructorIL.Emit(OpCodes.Ret);

                type = typeBuilder.CreateType();
                GeneratedControllerTypes.TryAdd(name, type);
            }

            return Activator.CreateInstance(type,
                BindingFlags.CreateInstance |
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.OptionalParamBinding,
                null, new object[] {context}, null) as GenericController<TEntity, TGet, TPost, TPut>;
        }

        private static readonly AssemblyBuilder AssemblyBuilder =
            AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("GenericControllers"),
                AssemblyBuilderAccess.Run);

        private static readonly ModuleBuilder ModuleBuilder =
            AssemblyBuilder.DefineDynamicModule("GenericControllers.dll");
    }
}