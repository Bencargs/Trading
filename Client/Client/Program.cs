using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            var stocks = client.GetStocks();
        }

        public class Order
        {
            public string Symbol { get; set; }
            public decimal Price { get; set; }
        }

        public enum OrderType
        {
            None = 0,
            Buy = 1
        }

        public class Client
        {
            private readonly string _url = @"localhost";
            private readonly int _port = 53139;

            public Client()
            {

            }

            public HashSet<string> GetStocks()
            {
                var requestUrl = $"http://{_url}:{_port}/api/orders"; ;
                var response = GetRequest(requestUrl);
                var stocks = JsonConvert.DeserializeObject<HashSet<string>>(response);
                return stocks;
            }

            public string PostOrder(OrderType type, Order order)
            {
                //replace with httpClient
                var data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(order));

                var request = (HttpWebRequest)WebRequest.Create(@"...");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                var response = request.GetResponse() as HttpWebResponse;
                using (var stream = response.GetResponseStream())
                {
                    return new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
            }

            private string GetRequest(string requestUrl)
            {
                var request = WebRequest.Create(requestUrl) as HttpWebRequest;

                var response = request.GetResponse() as HttpWebResponse;
                using (var stream = response.GetResponseStream())
                {
                    return new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
            }
        }
    }
}
