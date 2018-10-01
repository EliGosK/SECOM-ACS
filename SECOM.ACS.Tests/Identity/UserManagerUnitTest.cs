using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SECOM.ACS.Identity;

namespace SECOM.ACS.Tests
{
    [TestClass]
    public class UserManagerUnitTest
    {
        public ApplicationUserManager userManager { get; set; }
        public ApplicationRoleManager roleManager { get; set; }

        [TestInitialize]
        public void Init()
        {
            var db = new IdentityContext();
            
            //var manager = new ApplicationUserManager(new UserStore(db));
            userManager = new ApplicationUserManager(new UserStore(db));
            roleManager = new ApplicationRoleManager(new RoleStore(db));

            db.Seed();
        }

        [TestMethod]
        public void CreateRole()
        {
            var role = new Role() { Name = "Test", IsSystemRole = false, IsActive = true, CreateBy = "sittichok", UpdateBy = "sittichok" };
            roleManager.CreateAsync(role);
        }


        [TestMethod]
        public void CreateUser()
        {
            var user = new User() { UserName = "teerachot", FactoryCode= "F01", IsVerifyItemIn = true, IsVerifyItemOut = true, CreateBy= "sittichok", CreateDate =  DateTime.Now, UpdateBy = "sittichok", UpdateDate = DateTime.Now };
            var role = roleManager.FindByNameAsync("SystemAdmin").Result;
            user.Roles.Add(role);
            var result = userManager.CreateAsync(user,"1234567");
            if (!result.Result.Succeeded)
            {
                throw new Exception("Create user failed");
            }
            Console.WriteLine("Create user succeeded");
        }

        [TestMethod]
        public void FindUser()
        {
            var result = userManager.FindAsync("teerachot", "1234567");
            result.Wait();
            var user = result.Result;
            Assert.IsTrue(user != null && user.UserName == "sareeras");
            
        }
    }
}
