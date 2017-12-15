using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fosol.Core.Extensions.Types
{
    public static class TypeExtensions
    {
        public static bool IsEnumerable(this Type type)
        {
            return type.GetInterface(nameof(IEnumerable)) != null;
        }

        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }

        public static Type GetGenericType(this Type type)
        {
            return type.GetGenericArguments()[0];
        }

        public static Type[] GetGenericTypes(this Type type)
        {
            return type.GetGenericArguments();
        }
    }
}
