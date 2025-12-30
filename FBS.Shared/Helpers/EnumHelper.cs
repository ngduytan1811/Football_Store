namespace FBS.Shared.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using FBS.Shared.Utilities;
    using FBS.Shared.DataTranferObjects.Base;

    public static class EnumHelper<T>
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (var fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }

            return enumValues;
        }

        public static T? ToEnum(string value)
        {
            if (Enum.TryParse(typeof(T), value, true, out object result))
            {
                return (T)result;
            }

            return default;
        }

        public static T? ToEnum(int? value)
        {
            if (!value.HasValue || !Enum.IsDefined(typeof(T), value))
            {
                return default;
            }

            return ToEnum(Enum.GetName(typeof(T), value));
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(obj)).ToList();
        }

        public static string? GetDisplayValue(T? value)
        {
            if (value == null)
            {
                return null;
            }

            var fieldName = value?.ToString();

            if (string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            return GetDisplayValue(fieldName);
        }

        public static string? GetDisplayValue(int? value)
        {
            if (!value.HasValue || !Enum.IsDefined(typeof(T), value))
            {
                return string.Empty;
            }

            var fieldName = ((T)(object)value).ToString();

            if (string.IsNullOrEmpty(fieldName))
            {
                return string.Empty;
            }

            return GetDisplayValue(fieldName);
        }

        public static string? GetDisplayValue(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var fieldInfo = typeof(T).GetField(value);

            if (fieldInfo == null
                || fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) is not DisplayAttribute[] descriptionAttributes)
            {
                return string.Empty;
            }

            if (descriptionAttributes.Length > 0 && descriptionAttributes[0].ResourceType != null)
            {
                return FunctionDataUtils.LookupResource(
                    descriptionAttributes[0].ResourceType,
                    descriptionAttributes[0].Name ?? string.Empty);
            }

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Name : value;
        }

        public static IList<T> GetEnums(int[] values)
        {
            return values.Cast<T>().ToList();
        }

        public static List<SelectListItemDto> ConvertToSelectList(bool includedAll)
        {
            if (typeof(T).BaseType != typeof(Enum))
            {
                throw new InvalidCastException();
            }

            var listItems = new List<SelectListItemDto>();

            var arr = Enum.GetValues(typeof(T)).Cast<Enum>().OrderBy(x => x.ToString());
            try
            {
                arr = Enum.GetValues(typeof(T)).Cast<Enum>()
                    .OrderBy(x => x.GetType().GetField(x.ToString()).GetCustomAttribute<DisplayAttribute>().Order);
            }
            catch (Exception)
            {
                arr = Enum.GetValues(typeof(T)).Cast<Enum>().OrderBy(x => x.ToString());
            }

            foreach (var enumValue in arr)
            {
                var order = 0;
                try
                {
                    order = enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttribute<DisplayAttribute>().Order;
                }
                catch (Exception)
                {
                }

                if (order > -1)
                {
                    if ((enumValue.ToString() == "All" || enumValue.ToString() == "Tất cả") && includedAll)
                    {
                        listItems.Add(new SelectListItemDto
                        {
                            Text = GetDisplayValue(Convert.ToInt32(enumValue)),
                            Value = Convert.ToInt32(enumValue).ToString(),
                        });
                    }
                    else if (enumValue.ToString() != "All" || enumValue.ToString() != "Tất cả")
                    {
                        listItems.Add(new SelectListItemDto
                        {
                            Text = GetDisplayValue(Convert.ToInt32(enumValue)),
                            Value = Convert.ToInt32(enumValue).ToString(),
                        });
                    }
                }
            }

            return listItems;
        }
    }
}
