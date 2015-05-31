using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericToolkit.Core
{
    internal static class TypeScanner
    {
        private static readonly Type[] Types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToArray();

        internal static IEnumerable<Type> All
        {
            get { return Types; }
        }
    }
}