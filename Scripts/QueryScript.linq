<Query Kind="Program">
  <Reference Relative="..\Client\packages\Newtonsoft.Json.12.0.1\lib\net45\Newtonsoft.Json.dll">C:\Source\Trading\Client\packages\Newtonsoft.Json.12.0.1\lib\net45\Newtonsoft.Json.dll</Reference>
  <Reference Relative="..\Market\MarketService\bin\System.Net.Http.dll">C:\Source\Trading\Market\MarketService\bin\System.Net.Http.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

void Main()
{
    var client = new Client();
	client.GetSharePrice();
//    var stocks = client.GetStocks();
//	client.GetS("ABC");
}

public class Order
{
    public string Symbol { get; set; }
    public decimal Price { get; set; }
}

public class Client
{
    private readonly string _url = @"localhost";
    private readonly int _port = 52179;

	public void GetSharePrice()
	{
		var obj = new Stock { Ticker = "ABC" };
	    var request = CreatePostRequest("shares/price", obj);
		var prices = GetReply<Trade[]>(request);
		prices.Chart(x => x.Timestamp, y => y.Price, LINQPad.Util.SeriesType.Line).Dump();

		var obj2 = new Order
		{
		};
		var request2 = CreatePostRequest("shares/buy", obj2);
		var reply = GetReply<FillDetail>(request2);
		reply.Dump();
	}
	
	private HttpWebRequest CreatePostRequest(string endpoint, object obj)
	{
		var objBytes = Encoding.UTF8.GetBytes(obj.ToString());
	    string URI = $"http://{_url}:{_port}/api/{endpoint}";
	    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(URI, UriKind.RelativeOrAbsolute));
	    request.Method = "POST";
	    request.ContentType = "application/x-www-form-urlencoded";
	    request.Headers["applicationKey"] = "UFakeKkrayuAeVnoVAcjY54545455544";
	    request.ContentLength = objBytes.Length;
	
	    using (Stream stream = request.GetRequestStream())
	    {
	        stream.Write(objBytes, 0, objBytes.Length);
	    }
		
		return request;
	}
	
	private T GetReply<T>(HttpWebRequest request)
	{
		var jsonReply = "";
	    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
	    using (Stream stream = response.GetResponseStream())
	    using (StreamReader reader = new StreamReader(stream))
	    {
	        jsonReply = reader.ReadToEnd();
	    }
		
		return JsonConvert.DeserializeObject<T>(jsonReply); 
	}
	
	public class Trade
	{
		public DateTime Timestamp {get; set;}
		public int Volumne {get; set;}
		public decimal Price {get; set;}
	}
	
	public class PriceRequest
	{
		public DateTime From {get; set;}
		public Stock Stock {get; set;}
	}
	
	public class Stock
	{
		public string Ticker {get; set;}
	}
	
	public class FillDetail
    {
    }
}

// Define other methods and classes here