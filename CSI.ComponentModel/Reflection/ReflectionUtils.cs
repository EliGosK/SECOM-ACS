using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CSI.Utilities;
using CSI.Collections;

namespace CSI.Reflection
{
    public static class ReflectionUtils
    {
        public static Type GetObjectType(object v)
        {
            return (v != null) ? v.GetType() : null;
        }
        
        public static bool IsNullable<T>(T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }
       
        private static bool IsSubClassInternal(Type initialType, Type currentType, Type check, out Type implementingType)
        {
            if (currentType == check)
            {
                implementingType = currentType;
                return true;
            }

            // don't get interfaces for an interface unless the initial type is an interface
            if (check.IsInterface && (initialType.IsInterface || currentType == initialType))
            {
                foreach (Type t in currentType.GetInterfaces())
                {
                    if (IsSubClassInternal(initialType, t, check, out implementingType))
                    {
                        // don't return the interface itself, return it's implementor
                        if (check == implementingType)
                            implementingType = currentType;

                        return true;
                    }
                }
            }

            if (currentType.IsGenericType && !currentType.IsGenericTypeDefinition)
            {
                if (IsSubClassInternal(initialType, currentType.GetGenericTypeDefinition(), check, out implementingType))
                {
                    implementingType = currentType;
                    return true;
                }
            }

            if (currentType.BaseType == null)
            {
                implementingType = null;
                return false;
            }

            return IsSubClassInternal(initialType, currentType.BaseType, check, out implementingType);
        }

      
        /// <summary>
        /// Determines whether the specified MemberInfo can be read.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be read.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be read; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanReadMemberValue(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return true;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).CanRead;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be set.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be set.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be set; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanSetMemberValue(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return true;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).CanWrite;
                default:
                    return false;
            }
        }

        public static List<MemberInfo> GetFieldsAndProperties<T>(BindingFlags bindingAttr)
        {
            return GetFieldsAndProperties(typeof(T), bindingAttr);
        }

        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
        {
            List<MemberInfo> targetMembers = new List<MemberInfo>();

            targetMembers.AddRange(type.GetFields(bindingAttr));
            targetMembers.AddRange(type.GetProperties(bindingAttr));

            return targetMembers;
        }

        


        public static T CloneObjectProperties<TSource, T>(TSource value)
            where T : new()
        {
            T entity = new T();
            PropertyInfo[] props = typeof(TSource).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (!prop.CanRead) { continue; }
                PropertyInfo targetProp = typeof(T).GetProperty(prop.Name);
                if (targetProp == null || !targetProp.CanWrite || !IsPrimitiveTypeOrDecimalOrString(targetProp.PropertyType)) { continue; }

                targetProp.SetValue(entity, prop.GetValue(value, null), null);
            }
            return entity;
        }

        public static void CloneObjectProperties<T>(T source,T output)
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (!prop.CanRead) { continue; }
                PropertyInfo targetProp = typeof(T).GetProperty(prop.Name);
                if (targetProp == null || !targetProp.CanWrite || !IsPrimitiveTypeOrDecimalOrString(targetProp.PropertyType)) { continue; }

                targetProp.SetValue(output, prop.GetValue(source, null), null);
            }
        }

        public static bool IsPrimitiveTypeOrDecimalOrString(Type type) 
        {
            return type.IsPrimitive || type == typeof(String) || type.IsValueType; // == typeof(Decimal) || type == typeof(String) || type == typeof(DateTime) || type == typeof(TimeSpan);
        }

        #region Merge From ReflectionUtils Class
        private const BindingFlags CommonFlags = BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// 
        /// </summary>
        public static object CreateInstance(Type type, params object[] args)
        {
            return InvokeMember(
                type, null, null,
                CommonFlags | BindingFlags.CreateInstance | BindingFlags.Instance, args);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetField(object target, string fieldName, object value)
        {
            InvokeMember(
                target.GetType(), target, fieldName,
                CommonFlags | BindingFlags.SetField | BindingFlags.Instance, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static object GetField(object target, string fieldName)
        {
            return InvokeMember(
                target.GetType(), target, fieldName,
                CommonFlags | BindingFlags.GetField | BindingFlags.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetProperty(object target, string propertyName, object value)
        {
            InvokeMember(
                target.GetType(), target, propertyName,
                CommonFlags | BindingFlags.SetProperty | BindingFlags.Instance, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static object GetProperty(object target, string propertyName)
        {
            return InvokeMember(
                target.GetType(), target, propertyName,
                CommonFlags | BindingFlags.GetProperty | BindingFlags.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StaticSetField(Type type, string fieldName, object value)
        {
            InvokeMember(
                type, null, fieldName,
                CommonFlags | BindingFlags.SetField | BindingFlags.Static, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static object StaticGetField(Type type, string fieldName)
        {
            return InvokeMember(
                type, null, fieldName,
                CommonFlags | BindingFlags.GetField | BindingFlags.Static);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StaticSetProperty(Type type, string propertyName, object value)
        {
            InvokeMember(
                type, null, propertyName,
                CommonFlags | BindingFlags.SetProperty | BindingFlags.Static, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static object StaticGetProperty(Type type, string propertyName)
        {
            return InvokeMember(
                type, null, propertyName,
                CommonFlags | BindingFlags.GetProperty | BindingFlags.Static);
        }

        /// <summary>
        /// 
        /// </summary>
        public static object CallMethod(object target, string methodName, params object[] args)
        {
            return InvokeMember(
                target.GetType(), target, methodName,
                CommonFlags | BindingFlags.InvokeMethod | BindingFlags.Instance, args);
        }

        /// <summary>
        /// 
        /// </summary>
        public static object StaticCallMethod(Type type, string memberName, params object[] args)
        {
            return InvokeMember(
                type, null, memberName,
                CommonFlags | BindingFlags.InvokeMethod | BindingFlags.Static, args);
        }

        /// <summary>
        /// 
        /// </summary>
        private static object InvokeMember(
            Type type, object target, string memberName, BindingFlags flags, params object[] args)
        {
            return type.InvokeMember(memberName, flags, null, target, args);
        }

        /// <summary>
        /// Gets the value of the property with the specified name.
        /// </summary>
        /// <param name="item">An object instance.</param>
        /// <param name="name">The property name.</param>
        /// <returns>The property value.</returns>
        public static object GetPropertyValue(object item, string name)
        {
            PropertyInfo property = GetPropertyInfo(item, name);
            return (property != null && property.CanRead) ? property.GetValue(item, null) : null;
        }

        /// <summary>
        /// Gets a <see cref="PropertyInfo"/> object representing the property
        /// belonging to the object having the specified name.
        /// </summary>
        /// <param name="item">An object instance.</param>
        /// <param name="name">The property name.</param>
        /// <returns>A <see cref="PropertyInfo"/> object, or null if the object
        /// instance does not have a property with the specified name.</returns>
        public static PropertyInfo GetPropertyInfo(object item, string name)
        {
            return (item != null) ? GetPropertyInfo(item.GetType(), name) : null;
        }

        /// <summary>
        /// Gets a <see cref="PropertyInfo"/> object representing the property
        /// belonging to the runtime type having the specified name.
        /// </summary>
        /// <param name="type">The runtime type.</param>
        /// <param name="name">The property name.</param>
        /// <returns>A <see cref="PropertyInfo"/> object, or null if the runtime
        /// type does not have a property with the specified name.</returns>
        public static PropertyInfo GetPropertyInfo(Type type, string name)
        {
            return type != null && !string.IsNullOrEmpty(name) ? type.GetProperty(name) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesWithAttribute(Type attributeType, string assemblyName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            return GetTypesWithAttribute(attributeType, assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesWithAttribute(Type attributeType, Assembly assembly)
        {
            List<Type> list = new List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsDefined(attributeType, false))
                    list.Add(type);
            }

            return list;
        }
        #endregion

        
    }
}