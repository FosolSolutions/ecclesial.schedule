using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Fosol.Core.Extensions.PropertyInfos
{
    public static class PropertyExtensions
    {
        public static DescriptionAttribute GetDescription(this PropertyInfo property)
        {
            return property.GetCustomAttribute<DescriptionAttribute>();
        }
    }
}
