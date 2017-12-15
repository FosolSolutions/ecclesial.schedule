﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Fosol.Core.Extensions.PropertyInfos;

namespace Fosol.Core.Extensions.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
