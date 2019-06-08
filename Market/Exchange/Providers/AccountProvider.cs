using Contracts.Models;
using Contracts.Providers;
using System;
using System.Collections.Generic;

namespace Exchange.Providers
{
    public class AccountProvider : IAccountProvider
    {
        private Dictionary<Guid, User> _users = new Dictionary<Guid, User>();
        private readonly IBankingProvider _bankingProvider;
        private readonly IHoldingsProvider _holdingsProvider;

        public AccountProvider(
            IBankingProvider bankingProvider,
            IHoldingsProvider holdingsProvider)
        {
            _bankingProvider = bankingProvider;
            _holdingsProvider = holdingsProvider;
        }

        public Guid RegisterUser(string username)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = username
            };
            _users.Add(user.UserId, user);
            _holdingsProvider.CreateUser(user);
            _bankingProvider.CreateAccount(user);
            return user.UserId;
        }

        public User GetUser(Guid userId)
        {
            _users.TryGetValue(userId, out User user);
            return user;
        }
    }
}
