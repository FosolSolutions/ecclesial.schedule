using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fosol.Core.Extensions.IEnumerables
{
    public static class IEnumerableExtensions
    {
        public static Type GetGenericType<T>(this IEnumerable<T> obj)
        {
            return typeof(T);
        }

        public static void ForEach<T>(this IEnumerable<T> obj, Action<T> action)
        {
            foreach (var item in obj)
            {
                action(item);
            }
        }
    }
}
