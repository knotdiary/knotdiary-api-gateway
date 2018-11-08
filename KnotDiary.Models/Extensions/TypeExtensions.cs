using System;
using System.Reflection;
using System.Collections.Generic;

namespace KnotDiary.Common.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetInheritanceHierarchy(this Type type)
        {
            for (var current = type; current != null; current = current.GetTypeInfo().BaseType)
                yield return current;
        }
    }
}
