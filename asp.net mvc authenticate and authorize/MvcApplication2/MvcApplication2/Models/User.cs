using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomAuthorize.Models
{
    public class User
    {
        public int UserId { get; set; }

        public String Username { get; set; }

        public String Email { get; set; }

        public String FirstName { get; set; }
        public String LastName { get; set; }

        public Boolean IsActive { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}