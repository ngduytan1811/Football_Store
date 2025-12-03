using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FBS.Shared.Helpers   
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString());
            if (member.Length > 0)
            {
                var attribute = member[0].GetCustomAttribute<DisplayAttribute>();
                if (attribute != null)
                {
                    return attribute.GetName();
                }
            }
            return enumValue.ToString();
        }
    }
}
