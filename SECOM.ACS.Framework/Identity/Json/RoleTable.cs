using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SECOM.ACS.Identity.Json
{
    public class RoleTable
    {
        private readonly string _file;

        public RoleTable(string file) 
        {
            this._file = file;    
        }

        public int Insert(IdentityRole role)
        {
            return 1;
        }

        public int Delete(IdentityRole role)
        {
            return 1;
        }

        public int Update(IdentityRole role)
        {
            return 1;
        }

        public IdentityRole GetRoleById(int roleId)
        {
            return GetDataFromFile().Where(t => t.Id == roleId).FirstOrDefault();
        }

        public IdentityRole GetRoleByName(string roleName)
        {
            return GetDataFromFile().Where(t => t.Name == roleName).FirstOrDefault();
        }

        public IEnumerable<IdentityRole> GetRoles()
        {
            return GetDataFromFile();
        }

        private List<IdentityRole> GetDataFromFile()
        {
            return JsonConvert.DeserializeObject<List<IdentityRole>>(File.ReadAllText(this._file));
        }
    }
}
