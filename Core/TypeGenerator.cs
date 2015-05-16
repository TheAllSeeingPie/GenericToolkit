using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace GenericToolkit.Core
{
    internal static class TypeGenerator
    {
        private static readonly ConcurrentDictionary<Type, Type> GeneratedTypes = new ConcurrentDictionary<Type, Type>();

        internal static T Generate<T>() where T : class
        {
            var type = typeof (T);

            if (type.IsInterface)
            {
                return (T) Generate(type);
            }

            GeneratedTypes.TryAdd(type, type);
            return (T)Activator.CreateInstance(type);
        }

        internal static object Generate(Type type)
        {
            Type generatedType;

            if (!GeneratedTypes.TryGetValue(type, out generatedType))
            {
                var name = type.FullName.Replace('.', '_').Replace('+', '_');
                var typeBuilder = ModuleBuilder.DefineType(name, TypeAttributes.Public
                                                                 | TypeAttributes.Class
                                                                 | TypeAttributes.AutoClass
                                                                 | TypeAttributes.AnsiClass
                                                                 | TypeAttributes.Serializable
                                                                 | TypeAttributes.BeforeFieldInit);

                typeBuilder.AddInterfaceImplementation(type);

                var interfaces = FindInterfaces(type);
                var properties = interfaces.SelectMany(t => t.GetProperties());
                var methods = interfaces.SelectMany(t => t.GetMethods());
                foreach (var property in properties)
                {
                    var fieldBuilder = typeBuilder.DefineField(string.Format("_{0}", property.Name),
                        property.PropertyType, FieldAttributes.Private);
                    var propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.HasDefault,
                        property.PropertyType, null);
                    const MethodAttributes getSetAttrs =
                        MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName |
                        MethodAttributes.HideBySig;

                    var getterName = string.Format("get_{0}", property.Name);
                    var getMethodBuilder = typeBuilder.DefineMethod(getterName, getSetAttrs,
                        property.PropertyType, Type.EmptyTypes);
                    var getIL = getMethodBuilder.GetILGenerator();
                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                    getIL.Emit(OpCodes.Ret);
                    propertyBuilder.SetGetMethod(getMethodBuilder);
                    typeBuilder.DefineMethodOverride(getMethodBuilder, methods.Single(m => m.Name == getterName));

                    var setterName = string.Format("set_{0}", property.Name);
                    var setMethodBuilder = typeBuilder.DefineMethod(setterName, getSetAttrs,
                        null, new[] {property.PropertyType});
                    var setIL = setMethodBuilder.GetILGenerator();
                    setIL.Emit(OpCodes.Ldarg_0);
                    setIL.Emit(OpCodes.Ldarg_1);
                    setIL.Emit(OpCodes.Stfld, fieldBuilder);
                    setIL.Emit(OpCodes.Ret);
                    propertyBuilder.SetSetMethod(setMethodBuilder);
                    typeBuilder.DefineMethodOverride(setMethodBuilder, methods.Single(m => m.Name == setterName));
                }

                generatedType = typeBuilder.CreateType();
                GeneratedTypes.TryAdd(type, generatedType);
            }

            return Activator.CreateInstance(generatedType);
        }

        internal static Type GetGeneratedType<T>()
        {
            return GetGeneratedType(typeof (T));
        }

        internal static Type GetGeneratedType(Type type)
        {
            Type generatedType;
            if (!GeneratedTypes.TryGetValue(type, out generatedType))
            {
                generatedType = Generate(type).GetType();
            }
            return generatedType;
        }

        private static Type[] FindInterfaces(Type type)
        {
            return type.GetInterfaces().Any()
                ? new[] {type}.Concat(type.GetInterfaces().SelectMany(FindInterfaces)).ToArray()
                : new[] {type};
        }

        private static readonly AssemblyBuilder AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("GenericTypes"), AssemblyBuilderAccess.Run);
        private static readonly ModuleBuilder ModuleBuilder = AssemblyBuilder.DefineDynamicModule("GenericTypes.dll");
    }
}