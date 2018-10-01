using System;

namespace SECOM.ACS.Identity.Json
{
    public class UserLoginsTable
    {
        private readonly string _file;

        /// <summary>
        /// Constructor that takes a Oracle Database instance 
        /// </summary>
        /// <param name="database"></param>
        public UserLoginsTable(string file)
        {
            this._file = file;
        }

        /// <summary>
        /// Returns a list of user's logins
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public IdentityUser GetBySId(string sid)
        {
            throw new NotImplementedException();    
        }

       
    }
}
