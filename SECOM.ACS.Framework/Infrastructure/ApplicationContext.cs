using Newtonsoft.Json;
using SECOM.ACS.Caching;
using SECOM.ACS.Configuration;
using SECOM.ACS.Identity;
using SECOM.ACS.Models;
using SECOM.ACS.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Caching;
using System.Web.Hosting;

namespace SECOM.ACS.Infrastructure
{
    public class ApplicationContext
    {
        public static AppSettingsContainer Setting
        {
            get
            {
                var env = ConfigurationManager.AppSettings["EnvironmentName"] ?? "";
                var filePath = HostingEnvironment.MapPath(String.IsNullOrEmpty(env) ? "~/appsettings.json" : $"~/appsettings.{env}.json");
                return ApplicationCachingManager.Get<AppSettingsContainer>("setting", () => {
                    if (File.Exists(filePath))
                    {
                        return JsonConvert.DeserializeObject<AppSettingsContainer>(File.ReadAllText(filePath));
                    }
                    return new AppSettingsContainer();
                },() => filePath);
            }
        }

        private static DataCacheContext _dataContext;
        public static DataCacheContext DataContext
        {
            get
            {
                if (_dataContext == null)
                {
                    _dataContext = new DataCacheContext();
                }
                return _dataContext;
            }
        }

        #region Security
        private static AppSecurityContext _securityContext;
        public static AppSecurityContext SecurityContext
        {
            get
            {
                if (_securityContext == null)
                {
                    _securityContext = new AppSecurityContext();
                }
                return _securityContext;
            }
        }
        #endregion

        
    }
}
