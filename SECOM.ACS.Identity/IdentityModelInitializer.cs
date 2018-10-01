using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Identity
{
    public partial class IdentityContext
    {
        public void Seed()
        {
            if (!this.Roles.Any() && !this.Users.Any())
            {
                var adminRole = this.Roles.Add(new Role("SystemAdmin") { IsSystemRole = true, IsActive = true, CreateBy = "system", CreateDate = DateTime.Now, UpdateBy = "system", UpdateDate = DateTime.Now });
                var officerRole = this.Roles.Add(new Role("Officer") { IsSystemRole = true, IsActive = true, CreateBy = "system", CreateDate = DateTime.Now, UpdateBy = "system", UpdateDate = DateTime.Now });
                var securityRole = this.Roles.Add(new Role("Security") { IsSystemRole = true, IsActive = true, CreateBy = "system", CreateDate = DateTime.Now, UpdateBy = "system", UpdateDate = DateTime.Now });
                var gaRole = this.Roles.Add(new Role("GA") { IsSystemRole = true, IsActive = true, CreateBy = "system", CreateDate = DateTime.Now, UpdateBy = "system", UpdateDate = DateTime.Now });

                var user1 = new User()
                {
                    UserName = "sittichok",
                    Password = "470262",
                    PasswordFormat = 0,
                    IsVerifyItemIn = true,
                    IsVerifyItemOut = true,
                    FactoryCode = "F01",
                    CreateBy = "system",
                    CreateDate = DateTime.Now,
                    UpdateBy = "system",
                    UpdateDate = DateTime.Now
                };
                user1.Roles.Add(adminRole);
                user1.Roles.Add(officerRole);
                this.Users.Add(user1);

                var user2 = new User()
                {
                    UserName = "thanawit",
                    Password = "1234567",
                    PasswordFormat = 0,
                    IsVerifyItemIn = true,
                    IsVerifyItemOut = true,
                    FactoryCode = "F01",
                    CreateBy = "system",
                    CreateDate = DateTime.Now,
                    UpdateBy = "system",
                    UpdateDate = DateTime.Now
                };
                user2.Roles.Add(adminRole);
                user2.Roles.Add(officerRole);
                this.Users.Add(user2);

                var user3 = new User()
                {
                    UserName = "teerachot",
                    Password = "1234567",
                    PasswordFormat = 0,
                    IsVerifyItemIn = true,
                    IsVerifyItemOut = true,
                    FactoryCode = "F01",
                    CreateBy = "system",
                    CreateDate = DateTime.Now,
                    UpdateBy = "system",
                    UpdateDate = DateTime.Now
                };
                user3.Roles.Add(adminRole);
                user3.Roles.Add(officerRole);
                this.Users.Add(user3);

                this.SaveChanges();
            }
            
        }
    }
}
