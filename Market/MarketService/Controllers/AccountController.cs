using Contracts.Models;
using Contracts.Providers;
using Exchange.Services;
using System;
using System.Web.Http;

namespace MarketService.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly Account _accounts;
        private readonly IBankingProvider _bankingProvider;
        private readonly IHoldingsProvider _holdingsProvider;

        public AccountController(
            Account accounts,
            IBankingProvider bankingProvider,
            IHoldingsProvider holdingsProvider)
        {
            _accounts = accounts;
            _bankingProvider = bankingProvider;
            _holdingsProvider = holdingsProvider;
        }


        [HttpGet]
        public IHttpActionResult GetUser([FromBody] object jsonData)
        {
            var reply = _accounts.GetUser(Request.Headers);
            switch (reply.FailureReason)
            {
                case GetUserResponse.Reason.NoUserInHeader:
                    return BadRequest("Missing userId in request header");
                case GetUserResponse.Reason.InvalidUserId:
                    return BadRequest("Invalid user Id");
                case GetUserResponse.Reason.NotFound:
                    return NotFound();
                case GetUserResponse.Reason.None:
                    return Ok(reply.User);
                default:
                    return InternalServerError(new ArgumentOutOfRangeException());
            }
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult CreateUser([FromBody]string username)
        {
            var userId = _accounts.RegisterUser(Request.Headers, username);
            if (userId == null)
                return InternalServerError();

            return Ok(userId);
        }

        [HttpGet]
        public IHttpActionResult GetFunds([FromBody] object jsonData)
        {
            var reply = _accounts.GetUser(Request.Headers);
            if (reply.FailureReason != GetUserResponse.Reason.None)
                return BadRequest("Invalid User");

            var bankAccount = _bankingProvider.GetAccount(reply.User);
            if (bankAccount == null)
                return NotFound();

            return Ok(bankAccount.Balance);
        }

        [HttpPost]
        [Route("Funds")]
        public IHttpActionResult AddFunds([FromBody]decimal amount)
        {
            var reply = _accounts.GetUser(Request.Headers);
            if (reply.FailureReason != GetUserResponse.Reason.None)
                return BadRequest("Invalid User");

            _bankingProvider.AddFunds(reply.User, amount);

            return Ok();
        }

        [HttpGet]
        [Route("StockHeld")]
        public IHttpActionResult GetHolding([FromBody] Stock stock)
        {
            var reply = _accounts.GetUser(Request.Headers);
            if (reply.FailureReason != GetUserResponse.Reason.None)
                return BadRequest("Invalid User");

            var stockHeld = _holdingsProvider.GetHolding(reply.User, stock);

            return Ok(stockHeld);
        }

        [HttpGet]
        [Route("Holdings")]
        public IHttpActionResult GetHoldings([FromBody] object jsonData)
        {
            var reply = _accounts.GetUser(Request.Headers);
            if (reply.FailureReason != GetUserResponse.Reason.None)
                return BadRequest("Invalid User");

            var allHoldings = _holdingsProvider.GetHoldings(reply.User);

            return Ok(allHoldings);
        }
    }
}