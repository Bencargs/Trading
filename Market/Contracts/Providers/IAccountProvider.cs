using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Providers
{
    public interface IAccountProvider
    {
        Guid RegisterUser(string username);
        User GetUser(Guid userId);
    }
}
