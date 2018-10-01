using CSI.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSI.Utilities
{
    public static class AssemblyUtility
    {
        public static string FormatVersion(Version assemblyVersion)
        {
            if (assemblyVersion.Build == 0)
            {
                return (assemblyVersion.Major + "." + assemblyVersion.Minor);
            }
            return string.Concat(new object[] { assemblyVersion.Major, ".", assemblyVersion.Minor, ".", assemblyVersion.Build });
        }

        public static Type[] GetClassWithAssignableFromBaseType(Assembly assembly, Type baseType)
        {
            List<Type> list = new List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if ((baseType.IsAssignableFrom(type) && type.IsClass) && !type.IsAbstract)
                {
                    list.Add(type);
                }
            }
            return list.ToArray();
        }         

        public static Type[] GetTypeFromAttribute<TAttr>(Assembly assembly, bool inherit) where TAttr: Attribute
        {
            List<Type> list = new List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsDefined(typeof(TAttr), inherit))
                {
                    list.Add(type);
                }
            }
            return list.ToArray();
        }

        public static Version GetVersion(Assembly assembly)
        {
            return assembly.GetName().Version;
        }
    }
}

