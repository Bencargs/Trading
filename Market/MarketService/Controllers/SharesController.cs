using Contracts.Models;
using Contracts.Providers;
using Contracts.Responses;
using Exchange.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MarketService.Controllers
{
    [RoutePrefix("api/Shares")]
    public class SharesController : ApiController
    {
        private readonly Market _market;
        private readonly Account _accounts;
        private readonly ISharesProvider _sharesProvider;
        private readonly IOrderProvider _ordersProvider;

        public SharesController(
            Market market,
            Account accounts,
            ISharesProvider sharesProvider,
            IOrderProvider ordersProvider)
        {
            _market = market;
            _accounts = accounts;
            _sharesProvider = sharesProvider;
            _ordersProvider = ordersProvider;
        }

        [HttpGet]
        [Route("Price/{ticker}")]
        public decimal GetPrice(string ticker)
        {
            return _sharesProvider.GetLastPrice(new Stock { Ticker = ticker });
        }

        [HttpPost]
        [Route("Trades")]
        public IHttpActionResult GetTrades([FromBody]Stock stock)
        {
            var user = _accounts.GetUser(Request.Headers);
            if (user.FailureReason != GetUserResponse.Reason.None)
                return BadRequest("Invalid user");

            var trades = _sharesProvider.GetTrades(stock);
            return Ok(trades);
        }
        
        [HttpPost]
        [Route("Buy")]
        public IHttpActionResult Buy([FromBody]Order order)
        {
            var user = _accounts.GetUser(Request.Headers);
            if (user.FailureReason != GetUserResponse.Reason.None)
                return BadRequest("Invalid user");

            var reply = order.Type == OrderType.Market
                ? _market.MarketBuy(order.Owner, order.Stock, order.Quantity)
                : _market.LimitBuy(order.Owner, order.Stock, order.Quantity, order.Price ?? 0m);

            return Ok(reply);
        }

        [HttpPost]
        [Route("Sell")]
        public IHttpActionResult Sell([FromBody]Order order)
        {
            var user = _accounts.GetUser(Request.Headers);
            if (user.FailureReason != GetUserResponse.Reason.None)
                return BadRequest("Invalid user");

            var reply = order.Type == OrderType.Market
                ? _market.MarketSell(order.Owner, order.Stock, order.Quantity)
                : _market.LimitSell(order.Owner, order.Stock, order.Quantity, order.Price ?? 0m);

            return Ok(reply);
        }

        [HttpPost]
        [Route("Price")]
        public IHttpActionResult Post([FromBody] Stock stock)
        {
            var random = new Random();
            var startTime = new DateTime(2019, 5, 29, 10, 17, 23, 103);
            var startPrice = random.NextDouble() * 500;

            var maxValue = 50;
            var minValue = -50;
            var prices = Enumerable.Range(0, 1000).Select(x => new Trade
            {
                Timestamp = startTime.AddMilliseconds(x * 300),
                Volume = random.Next(0, 300),
                Price = (decimal) (startPrice += (random.NextDouble() * (maxValue - minValue) + minValue))
            });
            return Ok(prices);
        }
    }
}