using System;
using System.Text.RegularExpressions;

namespace GenericToolkit.Core.WebApi
{
    public static class TypeExtensions
    {
        /// <summary>
        ///     Used to extract a class name from an interface using a convention of removing the starting I from a pascal cased
        ///     Interface or do nothing if it's a class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetClassName(this Type type)
        {
            return type.IsClass
                ? type.Name
                : Regex.Match(type.Name, "I?([A-Z][A-z]*)", RegexOptions.Compiled).Groups[1].Value;
        }
    }
}