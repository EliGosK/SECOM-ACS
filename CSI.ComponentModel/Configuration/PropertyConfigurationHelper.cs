namespace CSI.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;

    public static class PropertyConfigurationHelper
    {
        public static void Configure<T>(T instance, NameValueCollection parameters) where T: class
        {
            Type type = typeof(T);
            foreach (string str in parameters.Keys)
            {
                foreach (PropertyInfo info in type.GetProperties())
                {
                    if (string.Compare(info.Name, str, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        info.SetValue(instance, Convert.ChangeType(parameters[str], info.PropertyType), null);
                        break;
                    }
                }
            }
        }
    }
}

