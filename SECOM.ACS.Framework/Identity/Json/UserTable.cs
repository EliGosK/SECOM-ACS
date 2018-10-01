using Newtonsoft.Json;
using SECOM.ACS.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SECOM.ACS.Identity.Json
{
    public class UserTable
    {
        private readonly string _file;

        public UserTable(string file) 
        {
            this._file = file;
        }

        public int Insert(IdentityUser user)
        {
            throw new NotImplementedException();
        }

        public int Delete(IdentityUser user)
        {
            throw new NotImplementedException();
        }

        public int Update(IdentityUser user)
        {
            return 1;
        }

        public IdentityUser GetUserById(string userId)
        {
            return GetDataFromFile().Where(t => String.Compare( t.Id , userId, true) ==0).FirstOrDefault();
        }

        public IdentityUser GetUserByName(string username)
        {
            return GetDataFromFile().Where(t => String.Compare(t.UserName , username,true)==0).FirstOrDefault();
        }

        public IEnumerable<IdentityUser> GetIdentityUsers()
        {
            return GetDataFromFile();
        }

        private IEnumerable<IdentityUser> GetDataFromFile()
        {
            return JsonDataContext.GetDataFromJsonFile<IdentityUser>(_file);
        }
    }
}
