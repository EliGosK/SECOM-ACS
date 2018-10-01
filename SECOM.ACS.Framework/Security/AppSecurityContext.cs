using Newtonsoft.Json;
using SECOM.ACS.Identity;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace SECOM.ACS.Security
{
    public class AppSecurityContext
    {
        private const string rolesKey = "AppSecurityContext_Roles";
        private const string userRoleMappingsKey = "AppSecurityContext_UserRoles";
        private const string permissionsKey = "AppSecurityContext_Permissions";
        private static ObjectCache cache = MemoryCache.Default;

        public IList<PermissionMapping> Permissions
        {
            get
            {
                var data = cache[permissionsKey] as List<PermissionMapping>;
                return data ?? new List<PermissionMapping>();
            }
        }

        public IList<Role> Roles
        {
            get
            {
                var data = cache[rolesKey] as List<Role>;
                return data ?? new List<Role>();
            }
        }

        public IList<UserRoleMapping> UserRoles
        {
            get
            {
                var data = cache[userRoleMappingsKey] as List<UserRoleMapping>;
                return data ?? new List<UserRoleMapping>();
            }
        }

        public bool IsAuthorize(string role, string objectId, string permissionName)
        {
            var r = this.Roles.Where(t => String.Compare(t.Name, role, true) == 0).FirstOrDefault();
            if (r == null) { return false; }
            return this.Permissions.Where(t => t.RoleID == r.RoleID && String.Compare(t.ObjectID, objectId, true) == 0 && String.Compare(t.PermissionName, permissionName, true) == 0).Any();
        }

        public bool IsUserAuthorize(string user, string objectId, string permissionName)
        {
            var userRole = this.UserRoles.Where(t => String.Compare(t.UserName, user, true) == 0).FirstOrDefault();
            if (userRole == null) { return false; }

            return this.Permissions.Where(t => userRole.Roles.Contains(t.RoleID) && String.Compare(t.ObjectID, objectId, true) == 0 && String.Compare(t.PermissionName, permissionName, true) == 0).Any();
        }

        public void AddData(IEnumerable<Role> roles, IEnumerable<UserRoleMapping> userRoles, IEnumerable<PermissionMapping> permissions)
        {
            // Roles
            RemoveDataFromCached(rolesKey);
            AddDataToCache(rolesKey, roles);
            // User Roles
            RemoveDataFromCached(userRoleMappingsKey);
            AddDataToCache(userRoleMappingsKey, userRoles);
            // Permission
            RemoveDataFromCached(permissionsKey);
            AddDataToCache(permissionsKey, permissions);
        }

        private void AddDataToCache(string key,object value)
        {
            var policy = new CacheItemPolicy()
            {
                //AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
                SlidingExpiration = TimeSpan.FromMinutes(60)
            };
            string fileDependency = HostingEnvironment.MapPath($"~/cache-dependency-{key.ToLowerInvariant()}.cache");
            var content = JsonConvert.SerializeObject(value);
            EnsureCreateCacheFileDependency(fileDependency, content);
            policy.ChangeMonitors.Add(new HostFileChangeMonitor(new string[] { fileDependency }));
            cache.Add(key, value, policy);
            
        }

        private void EnsureCreateCacheFileDependency(string file,string content)
        {
            if (!File.Exists(file))
            {
                using (var fs = File.CreateText(file))
                {
                    fs.WriteLine(content);
                }
            }
        }

        public void RemoveDataFromCached(String key)
        {
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
        }

        public bool IsExpired
        {
            get {
                return cache[userRoleMappingsKey] == null || cache[rolesKey] == null|| cache[permissionsKey] == null;
            }
        }

        public void Clear()
        {
            var keys = new string[] { userRoleMappingsKey, rolesKey, permissionsKey };
            foreach (var key in keys)
            {
                RemoveDataFromCached(key);
            }
        }
    }
}
