using Contracts.Models;
using Contracts.Providers;
using System;
using System.Linq;
using System.Net.Http.Headers;

namespace Exchange.Services
{
    public class Account
    {
        private IAccountProvider _accountProvider;

        public Account(IAccountProvider accountProvider)
        {
            _accountProvider = accountProvider;
        }

        public GetUserResponse GetUser(HttpHeaders header)
        {
            var reply = new GetUserResponse();

            if (!header.Contains("UserId"))
                reply.FailureReason = GetUserResponse.Reason.NoUserInHeader;
            else if (!Guid.TryParse(header.GetValues("UserId").FirstOrDefault(), out Guid userId))
                reply.FailureReason = GetUserResponse.Reason.InvalidUserId;
            else
            {
                var user = _accountProvider.GetUser(userId);
                if (user == null)
                {
                    reply.User = new User { UserId = userId };
                    reply.FailureReason = GetUserResponse.Reason.NotFound;
                }
                else
                    reply.User = user;
            }

            return reply;
        }

        public Guid? RegisterUser(HttpHeaders header, string username)
        {
            var response = GetUser(header);
            if (response.FailureReason != GetUserResponse.Reason.NotFound)
                return null;

            var userId = _accountProvider.RegisterUser(response.User.UserId, username);
            if (userId == Guid.Empty)
                return null;

            return userId;
        }
    }
}
