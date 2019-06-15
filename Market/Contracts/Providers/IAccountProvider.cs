using Contracts.Models;
using System;

namespace Contracts.Providers
{
    public interface IAccountProvider
    {
        Guid RegisterUser(Guid userId, string username);
        User GetUser(Guid userId);
    }
}
