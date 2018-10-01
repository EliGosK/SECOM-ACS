using System.Security.Claims;

namespace SECOM.ACS.Identity.Json
{
    /// <summary>
    /// Class that represents the UserClaims table in the Oracle Database
    /// </summary>
    public class UserClaimsTable
    {
        private readonly string _file;

        /// <summary>
        /// Constructor that takes a Oracle Database instance 
        /// </summary>
        /// <param name="database"></param>
        public UserClaimsTable(string file)
        {
            this._file = file;
        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserId(string userId)
        {
            var claims = new ClaimsIdentity();            
            return claims;
        }

        /// <summary>
        /// Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            return 1;
        }

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="claim">User's claim to be added</param>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        public int Insert(Claim claim, string userId)
        {
            return 1;
        }

        /// <summary>
        /// Deletes a claim from a user 
        /// </summary>
        /// <param name="user">The user to have a claim deleted</param>
        /// <param name="claim">A claim to be deleted from user</param>
        /// <returns></returns>
        public int Delete(IdentityUser user, Claim claim)
        {
            return 1;
        }
    }
}
