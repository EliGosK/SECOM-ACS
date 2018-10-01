using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace SECOM.ACS.Caching
{
    public class ApplicationCachingManager
    {
        private static ObjectCache cache = MemoryCache.Default;

        private static void Add(string key, object value)
        {
            Add(key, value, () => HostingEnvironment.MapPath($"~/cache-dependency-{key.ToLowerInvariant()}.cache"));
        }

       private static void Add(string key, object value,Func<string> predicate)
        {
            var policy = new CacheItemPolicy()
            {
                //AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
                SlidingExpiration = TimeSpan.FromMinutes(60)
            };
            string fileDependency = predicate.Invoke();
            var content = JsonConvert.SerializeObject(value);
            EnsureCreateCacheFileDependency(fileDependency, content);
            policy.ChangeMonitors.Add(new HostFileChangeMonitor(new string[] { fileDependency }));
            cache.Add(key, value, policy);
        }

        private static void EnsureCreateCacheFileDependency(string file, string content)
        {
            if (!File.Exists(file))
            {
                using (var fs = File.CreateText(file))
                {
                    fs.WriteLine(content);
                }
            }
        }

        public static T Get<T>(String key,Func<T> predicate)
            where T : class
        {
            if (cache.Contains(key))
            {
                return cache[key] as T;
            }

            if (predicate != null) {
                var data = predicate.Invoke();
                Add(key, data);
                return data;
            }
            return null;
        }

        public static T Get<T>(String key, Func<T> predicate,Func<string> dependencyPredicate)
           where T : class
        {
            if (cache.Contains(key))
            {
                return cache[key] as T;
            }

            if (predicate != null)
            {
                var data = predicate.Invoke();
                if (dependencyPredicate!=null)
                    Add(key, data, dependencyPredicate);
                else
                    Add(key, data);
                return data;
            }
            return null;
        }

        public static void Remove(String key)
        {
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
        }
    }
}
