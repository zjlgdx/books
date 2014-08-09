using CustomAuthorize.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAuthorize.Models
{
    public interface IUserRepository
    {
        User Authenticate(string userName);
    }
}
