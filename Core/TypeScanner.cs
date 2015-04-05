using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericToolkit.Core
{
    public static class TypeScanner
    {
        private static readonly Type[] Types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToArray();

        public static IEnumerable<Type> All
        {
            get { return Types; }
        }
    }
}