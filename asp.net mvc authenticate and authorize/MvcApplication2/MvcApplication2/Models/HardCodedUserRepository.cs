using System;

namespace CustomAuthorize.Models
{
    public class HardCodedUserRepository:IUserRepository
    {
        public User Authenticate(string userName)
        {
            // you would likely query the database here instead...
            if (string.Equals(userName, "YVAN-LAPTOP\\Yibang", StringComparison.OrdinalIgnoreCase))
                return new User { UserId = 1,Username = "yvan", Roles = new Role[] { new Role(){ RoleName = "User"}}};
            else
                return null;
        }
    }
}