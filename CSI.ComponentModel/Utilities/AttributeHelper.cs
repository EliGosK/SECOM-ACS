namespace CSI.Utilities
{
    using CSI.ComponentModel;
    using CSI.Enumerations;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class AttributeHelper
    {
        public static TAttr GetAttribute<T, TAttr>(bool inherit) where TAttr: Attribute
        {
            TAttr[] customAttributes = typeof(T).GetCustomAttributes(typeof(TAttr), inherit) as TAttr[];
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return customAttributes[0];
            }
            return default(TAttr);
        }

        public static T GetAttribute<T>(Type type, bool inherit) where T: Attribute
        {
            T[] customAttributes = type.GetCustomAttributes(typeof(T), inherit) as T[];
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return customAttributes[0];
            }
            return default(T);
        }

        public static T GetAttributeOnEnum<T, TEnum>(TEnum value, bool inherit) where T: class where TEnum: struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("TEnum is not enumeration type");
            }
            foreach (string str in Enum.GetNames(typeof(TEnum)))
            {
                if (value.Equals(Enum.Parse(typeof(TEnum), str)))
                {
                    FieldInfo field = typeof(TEnum).GetField(str);
                    if (field != null)
                    {
                        T[] customAttributes = field.GetCustomAttributes(typeof(T), inherit) as T[];
                        if ((customAttributes == null) || (customAttributes.Length <= 0))
                        {
                            break;
                        }
                        return customAttributes[0];
                    }
                }
            }
            return default(T);
        }

        public static T GetAttributeOnInstance<T>(ICustomAttributeProvider instance, bool inherit) where T: Attribute
        {
            T[] attributesOnInstance = GetAttributesOnInstance<T>(instance, inherit);
            if ((attributesOnInstance != null) && (attributesOnInstance.Length > 0))
            {
                return attributesOnInstance[0];
            }
            return default(T);
        }

        public static TAttr[] GetAttributes<T, TAttr>(bool inherit) where TAttr: Attribute
        {
            return (typeof(T).GetCustomAttributes(typeof(TAttr), inherit) as TAttr[]);
        }

        public static T[] GetAttributes<T>(Type type, bool inherit) where T: Attribute
        {
            return (type.GetCustomAttributes(typeof(T), inherit) as T[]);
        }

        public static T[] GetAttributesOnEnum<T, TEnum>(TEnum value, bool inherit) where T: class where TEnum: struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("TEnum is not enumeration type");
            }
            foreach (string str in Enum.GetNames(typeof(TEnum)))
            {
                if (value.Equals(EnumHelper.Parse<TEnum>(str)))
                {
                    FieldInfo field = typeof(TEnum).GetField(str);
                    if (field != null)
                    {
                        return (field.GetCustomAttributes(typeof(T), inherit) as T[]);
                    }
                }
            }
            return null;
        }

        public static T[] GetAttributesOnInstance<T>(ICustomAttributeProvider instance, bool inherit) where T: Attribute
        {
            return (instance.GetCustomAttributes(typeof(T), inherit) as T[]);
        }

        public static FieldInfo[] GetFields<T, TAttr>(bool inherit) where TAttr: Attribute
        {
            return GetFields<T, TAttr>(inherit, BindingFlags.Default);
        }

        public static FieldInfo[] GetFields<T, TAttr>(bool inherit, BindingFlags flags) where TAttr: Attribute
        {
            return GetFields<TAttr>(typeof(T), inherit, flags);
        }

        public static FieldInfo[] GetFields<TAttr>(Type type, bool inherit) where TAttr: Attribute
        {
            return GetFields<TAttr>(type, inherit, BindingFlags.Default);
        }

        public static FieldInfo[] GetFields<TAttr>(Type type, bool inherit, BindingFlags flags) where TAttr: Attribute
        {
            List<FieldInfo> list = new List<FieldInfo>();
            foreach (FieldInfo info in type.GetFields(flags))
            {
                TAttr[] customAttributes = info.GetCustomAttributes(typeof(TAttr), inherit) as TAttr[];
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    list.Add(info);
                }
            }
            return list.ToArray();
        }

        public static MemberInfo[] GetMembers<T, TAttr>(bool inherit) where T: class where TAttr: Attribute
        {
            return GetMembers<T, TAttr>(inherit, BindingFlags.Default);
        }

        public static MemberInfo[] GetMembers<T, TAttr>(bool inherit, BindingFlags flags) where T: class where TAttr: Attribute
        {
            return GetMembers<TAttr>(typeof(T), inherit, flags);
        }

        public static MemberInfo[] GetMembers<TAttr>(Type type, bool inherit) where TAttr: Attribute
        {
            return GetMembers<TAttr>(type, inherit, BindingFlags.Default);
        }

        public static MemberInfo[] GetMembers<TAttr>(Type type, bool inherit, BindingFlags flags) where TAttr: Attribute
        {
            List<MemberInfo> list = new List<MemberInfo>();
            foreach (MemberInfo info in type.GetMembers(flags))
            {
                TAttr[] customAttributes = info.GetCustomAttributes(typeof(TAttr), inherit) as TAttr[];
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    list.Add(info);
                }
            }
            return list.ToArray();
        }

        public static PropertyInfo[] GetProperties<T, TAttr>(bool inherit) where TAttr: Attribute
        {
            return GetProperties<T, TAttr>(inherit, BindingFlags.Default);
        }

        public static PropertyInfo[] GetProperties<T, TAttr>(bool inherit, BindingFlags flags) where TAttr: Attribute
        {
            return GetProperties<TAttr>(typeof(T), inherit, flags);
        }

        public static PropertyInfo[] GetProperties<TAttr>(Type type, bool inherit) where TAttr: Attribute
        {
            return GetProperties<TAttr>(type, inherit, BindingFlags.Default);
        }

        public static PropertyInfo[] GetProperties<TAttr>(Type type, bool inherit, BindingFlags flags) where TAttr: Attribute
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (PropertyInfo info in type.GetProperties(flags))
            {
                TAttr[] customAttributes = info.GetCustomAttributes(typeof(TAttr), inherit) as TAttr[];
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    list.Add(info);
                }
            }
            return list.ToArray();
        }
    }
}

