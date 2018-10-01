using CSI.Properties;
using CSI.Resources;
using CSI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CSI.Enumerations
{
    public class EnumHelper
    {
        public static string GetDisplayName<T>(object value) where T: struct
        {
            string name = GetName<T>(value);
            if (String.IsNullOrEmpty(name)) { return ""; }
            var attr = AttributeHelper.GetAttributeOnInstance<DisplayAttribute>(typeof(T).GetField(name), false);
            if (attr != null)
            {
                return attr.Name;
            }
            return name;
        }

        public static List<EnumItem> GetList<T>() where T: struct
        {
            return GetList<T>(new EnumItemComparer());
        }

        public static List<EnumItem> GetList<T>(IComparer<EnumItem> comparer) where T: struct
        {
            List<EnumItem> list = new List<EnumItem>();
            foreach (int num in Enum.GetValues(typeof(T)))
            {
                string str = Enum.GetName(typeof(T), num);
                FieldInfo field = typeof(T).GetField(str);
                var attributeOnInstance = AttributeHelper.GetAttributeOnInstance<IgnoreDisplayAttribute>(field, false);
                if ((attributeOnInstance == null) || !attributeOnInstance.Ignore)
                {
                    var attr = AttributeHelper.GetAttributeOnInstance<DisplayAttribute>(field, false);
                    if (attr == null)
                    {
                        list.Add(new EnumItem(str, str, num, list.Count));
                    }
                    else {
                        var order = attr.GetOrder();
                        list.Add(new EnumItem(str, attr.Name, num, order.HasValue? order.Value:0));
                    }
                }
            }
            return list;
        }

        public static List<EnumItem> GetList<T>(string groupName, IComparer<EnumItem> comparer)
        {
            List<EnumItem> list = new List<EnumItem>();
            foreach (int num in Enum.GetValues(typeof(T)))
            {
                string str = Enum.GetName(typeof(T), num);
                FieldInfo field = typeof(T).GetField(str);
                var attributeOnInstance = AttributeHelper.GetAttributeOnInstance<IgnoreDisplayAttribute>(field, false);
                if ((attributeOnInstance == null) || !attributeOnInstance.Ignore)
                {
                    var attr = AttributeHelper.GetAttributeOnInstance<DisplayAttribute>(field, false);
                    if ((attr != null) && (attr.GroupName == groupName))
                    {
                        list.Add(new EnumItem(str, attr.Name, num, attr.Order));
                    }
                }
            }
            return list;
        }

        public static string GetName<T>(object value) where T: struct
        {
            return Enum.GetName(typeof(T), value);
        }

        public static bool IsDefine<T>(object value)
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException(StringResourceHelper.GetString(typeof(CSI.Properties.Resources), "InvalidEnumType", new object[] { typeof(T).FullName }));
            }
            return Enum.IsDefined(typeof(T), value);
        }

        public static T Parse<T>(int value) where T: struct
        {
            string name = Enum.GetName(typeof(T), value);
            return (T) Enum.Parse(typeof(T), name);
        }

        public static T Parse<T>(string value) where T: struct
        {
            return (T) Enum.Parse(typeof(T), value);
        }

        public static T ParseByDisplayName<T>(string value) where T: struct
        {
            foreach (string str in Enum.GetNames(typeof(T)))
            {
                var attributeOnInstance = AttributeHelper.GetAttributeOnInstance<DisplayAttribute>(typeof(T).GetField(str), false);
                string str2 = (attributeOnInstance == null) ? str : attributeOnInstance.Name;
                if (str2 == value)
                {
                    return (T) Enum.Parse(typeof(T), str);
                }
            }
            throw new Exception("Can not found enum from display name.");
        }
    }
}

