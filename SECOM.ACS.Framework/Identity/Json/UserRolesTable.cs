using SECOM.ACS.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECOM.ACS.Identity.Json
{
    public class UserRolesTable
    {
        private readonly string _file;

        /// <summary>
        /// Constructor that takes a Oracle Database instance 
        /// </summary>
        /// <param name="database"></param>
        public UserRolesTable(string file)
        {
            this._file = file;
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public List<int> FindByUserId(string userId)
        {
            return JsonDataContext.GetDataFromJsonFile<IdentityUserRole>(_file)
                .Where(t => String.Compare(userId, t.UserId, true) == 0)
                .Select(t => t.RoleId)
                .ToList();                
        }

        /// <summary>
        /// Deletes all roles from a user in the UserRoles table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            return 1;
        }

        /// <summary>
        /// Inserts a new role for a user in the UserRoles table
        /// </summary>
        /// <param name="user">The User</param>
        /// <param name="roleId">The Role's id</param>
        /// <returns></returns>
        public int Insert(IdentityUser user, int roleId)
        {
            return 1;
        }
        
    }
}
