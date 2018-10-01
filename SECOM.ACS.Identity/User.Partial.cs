using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SECOM.ACS.Identity
{
    public partial class User : IUser<int>
    {
       public UserData GetUserData()
        {
            return new UserData
            {
                EmployeeID = this.EmpID,
                FactoryCode = this.FactoryCode,
                IsVerifyItemIn = this.IsVerifyItemIn,
                IsVerifyItemOut = this.IsVerifyItemOut,
                IsLending = this.IsLending
            };
        }
    }

    public class UserData
    {
        public string EmployeeID { get; set; }
        public string FactoryCode { get; set; }
        public bool IsVerifyItemIn { get; set; }
        public bool IsVerifyItemOut { get; set; }
        public bool IsLending { get; set; }

        public static UserData Empty()
        {
            return new UserData();
        }
    }

    public enum PasswordFormats
    {
        Clear,
        Hash
    }
}
