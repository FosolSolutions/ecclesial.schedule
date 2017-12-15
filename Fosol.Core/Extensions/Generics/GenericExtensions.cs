using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fosol.Core.Extensions.Generics
{
    public static class GenericExtensions
    {
        public static bool In<T>(this T obj, params T[] values)
        {
            return values.Contains(obj);
        }
    }
}
