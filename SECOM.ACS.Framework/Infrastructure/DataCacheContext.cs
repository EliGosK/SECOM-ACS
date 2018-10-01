using Newtonsoft.Json;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace SECOM.ACS.Infrastructure
{
    public class DataCacheContext
    {
        private static ObjectCache cache = MemoryCache.Default;
        private const string systemMiscKey = "DataCacheContext_SystemMisc";

        private void AddDataToCache(string key, object value)
        {
            var policy = new CacheItemPolicy()
            {
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
            };
            string fileDependency = HostingEnvironment.MapPath($"~/cache-dependency-{key.ToLowerInvariant()}.cache");
            var content = JsonConvert.SerializeObject(value);
            EnsureCreateCacheFileDependency(fileDependency, content);
            policy.ChangeMonitors.Add(new HostFileChangeMonitor(new string[] { fileDependency }));
            cache.Add(key, value, policy);

        }

        public void RemoveDataFromCached(String key)
        {
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
        }

        private void EnsureCreateCacheFileDependency(string file, string content)
        {
            if (!File.Exists(file))
            {
                using (var fs = File.CreateText(file))
                {
                    fs.WriteLine(content);
                }
            }
        }

        public IList<SystemMisc> SystemMiscs
        {
            get
            {
                var data = cache[systemMiscKey] as List<SystemMisc>;
                return data ?? new List<SystemMisc>();
            }
        }

        public void LoadSystemMisc(IEnumerable<SystemMisc> data)
        {
            RemoveDataFromCached(systemMiscKey);
            AddDataToCache(systemMiscKey, data);
        }
    }
}
