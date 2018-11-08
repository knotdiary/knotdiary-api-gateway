using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace KnotDiary.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string ToEnumString<T>(this T type, bool useAttribute = false)
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);

            if (useAttribute)
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                return enumMemberAttribute.Value;
            }

            return name;
        }

        public static string GetDescription(this Enum enumValue)
        {
            var attr = GetCustomAttribute<DescriptionAttribute>(enumValue);

            return attr == null ? enumValue.ToString() : attr.Description;
        }

        public static T GetCustomAttribute<T>(Enum enumValue)
        {
            return enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttributes(false).OfType<T>().FirstOrDefault();
        }
    }
}
