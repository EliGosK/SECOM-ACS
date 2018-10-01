using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace CSI.Configuration
{
    public static class RuntimeConfiguration
    {
        public static void AddConfigurationProperties(ConfigurationPropertyCollection collection, IEnumerable<ConfigurationProperty> properties)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            foreach (ConfigurationProperty property in properties)
            {
                collection.Add(property);
            }
        }

        public static T GetAppSetting<T>(string appSettingName) where T: IConvertible
        {
            return GetAppSettingInternal<T>(appSettingName, false, default(T));
        }

        public static T GetAppSetting<T>(string appSettingName, T defaultValue) where T: IConvertible
        {
            return GetAppSettingInternal<T>(appSettingName, true, defaultValue);
        }

        private static T GetAppSettingInternal<T>(string appSettingName, bool useDefaultOnUndefined, T defaultValue) where T: IConvertible
        {
            string str = ConfigurationManager.AppSettings[appSettingName];
            if (str != null)
            {
                return (T) Convert.ChangeType(str, typeof(T));
            }
            if (!useDefaultOnUndefined)
            {
                throw new Exception(string.Format("{0} not defined in appSettings.", appSettingName));
            }
            return defaultValue;
        }

        public static string GetAppSettings(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "key value can not be null or empty.");
            }
            System.Configuration.Configuration configuration = GetConfiguration();
            string str = null;
            KeyValueConfigurationElement element = configuration.AppSettings.Settings[key];
            if (element != null)
            {
                str = element.Value;
            }
            return str;
        }

        public static System.Configuration.Configuration GetConfiguration()
        {
            if (HttpContext.Current != null)
            {
                return WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            }
            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public static string GetConnectionString(string connectionStringName)
        {
            if (connectionStringName == null)
            {
                throw new ArgumentNullException("connectionStringName");
            }
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (settings == null)
            {
                throw new Exception(string.Format("No connection string settings with the name '{0}'.", connectionStringName));
            }
            return settings.ConnectionString;
        }

        public static string[] GetConnectionStringNames() 
        {
            List<string> names = new List<string>();
            for (int i = 0; i <  ConfigurationManager.ConnectionStrings.Count; i++)
            {
                names.Add(ConfigurationManager.ConnectionStrings[i].Name);
            }
            return names.ToArray();
        }
    }
}

