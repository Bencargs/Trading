using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MarketService.Controllers
{
    public class SharePriceController : ApiController
    {
        public IEnumerable<Trade> Post([FromBody]SharePriceRequest request)
        {
            var random = new Random();
            var startTime = new DateTime(2019, 5, 29, 10, 17, 23, 103);
            var startPrice = random.NextDouble() * 500;

            var maxValue = 50;
            var minValue = -50;
            var prices = Enumerable.Range(0, 1000).Select(x => new Trade
            {
                Timestamp = startTime.AddMilliseconds(100),
                Volume = random.Next(0, 300),
                Price = (decimal) (startPrice += (random.NextDouble() * (maxValue - minValue) + minValue))
            });
            return prices;
        }
    }

    public class SharePriceRequest
    {
        public DateTime From { get; set; }
        public Stock Stock { get; set; }
    }
}