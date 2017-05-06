using System;
using System.ComponentModel;
using System.Reflection;
using OneDriveRestAPI.Model;

namespace OneDriveRestAPI.Util
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
			/*
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute),false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
			*/
			var attr = value.GetType().GetTypeInfo().GetDeclaredField(value.ToString()).GetCustomAttribute(typeof(DescriptionAttribute))
							as DescriptionAttribute;
			if (attr != null)
			{
				return attr.Description;
			}
			return null;
		}
    }
}