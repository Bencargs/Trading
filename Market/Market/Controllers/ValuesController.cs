using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Exchange.Controllers
{
    public class OrdersController : ApiController
    {
        private HashSet<string> _stocks = new HashSet<string>
        {
            {"ABC"}
        };

        private class Order
        {
            //public DateTime Timestamp { get; set; }
            //public string Account { get; set; }
            public string Symbol { get; set; }
            public decimal Price { get; set; }
            //public int Volume { get; set; }
        }

        private Dictionary<string, List<decimal>> _history = new Dictionary<string, List<decimal>>
        {
            {"ABC", new List<decimal>() }
        };

        // GET api/orders
        public IEnumerable<string> Get()
        {
            return _stocks;
        }

        //// GET api/values/5
        public decimal Try(string symbol)
        {
            if (_history.TryGetValue(symbol, out List<decimal> prices))
            {
                return prices.LastOrDefault();
            }
            return 0;
        }

        //// POST api/values
        //public void Post([FromBody]string value)
        //{

        //}
        [HttpPost]
        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            var result = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var order = JsonConvert.DeserializeObject<Order>(result);

            if (_history.ContainsKey(order.Symbol))
            {
                _history[order.Symbol].Add(order.Price);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent($"Buy 1 {order.Symbol} @ {order.Price}")
                };
            }

            return null;
        }

        //// PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //public void Delete(int id)
        //{
        //}
    }
}
