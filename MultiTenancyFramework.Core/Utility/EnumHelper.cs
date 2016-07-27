using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MultiTenancyFramework
{
    public class EnumHelper
    {
        /// <summary>
        /// A class representing Name-Value pair for an enum. NB: The 'Value' is int
        /// </summary>
        public class NV
        {
            public string Name { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return string.Format("Name - {0}; Value - {1}.", Name, Value);
            }
        }

        public static List<NV> GetEnumNames(Type enumType)
        {
            return GetEnumNames(enumType, true);
        }

        public static List<NV> GetEnumNames<T>()
        {
            return GetEnumNames(typeof(T), true);
        }

        public static List<NV> GetEnumNames<T>(bool orderItems)
        {
            return GetEnumNames(typeof(T), orderItems);
        }

        public static List<NV> GetEnumNames<T>(IEnumerable enums)
        {
            return GetEnumNames(typeof(T), enums);
        }

        public static List<NV> GetEnumNames(Type type, IEnumerable enums)
        {
            return GetEnumNames(type, false, enums);
        }

        public static string GetEnumName<TEnum>(object enumValue)
        {
            return GetEnumName(typeof(TEnum), enumValue);
        }

        public static string GetEnumName(Type enumType, object enumValue)
        {
            string enumName = enumValue.ToString();
            int enumNumber;
            if (int.TryParse(enumName, out enumNumber))
            {
                enumName = Enum.GetName(enumType, Enum.Parse(enumType, enumName));
            }
            var nameAttribute = (EnumDescriptionAttribute[])enumType.GetField(enumName).GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

            return (nameAttribute == null || nameAttribute.Length == 0)
                ? enumName.AsSplitPascalCasedString()
                : nameAttribute[0].Name;
        }

        public static List<NV> GetEnumNames(Type enumType, bool orderItems, IEnumerable enums)
        {
            List<NV> nameValueList = new List<NV>();

            foreach (int enumValue in enums.Cast<int>())
            {
                nameValueList.Add(new NV { Name = GetEnumName(enumType, enumValue), Value = enumValue });
            }

            if (orderItems)
            {
                return nameValueList.OrderBy(n => n.Name).ToList();
            }
            return nameValueList;
        }

        public static List<NV> GetEnumNames(Type enumType, bool orderItems)
        {
            return GetEnumNames(enumType, orderItems, Enum.GetValues(enumType));
        }

        public static List<NV> GetEnumNames(string enumStringType)
        {
            return GetEnumNames(enumStringType, false);
        }

        public static List<NV> GetEnumNames(string enumStringType, bool orderItems)
        {
            Type enumType = Type.GetType(enumStringType);
            return GetEnumNames(enumType, orderItems);
        }

    }
}
