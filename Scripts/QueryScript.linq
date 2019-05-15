<Query Kind="Program">
  <Reference Relative="..\Client\packages\Newtonsoft.Json.12.0.1\lib\net45\Newtonsoft.Json.dll">C:\Source\Trading\Client\packages\Newtonsoft.Json.12.0.1\lib\net45\Newtonsoft.Json.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

void Main()
{
    var client = new Client();
    var stocks = client.GetStocks();
	client.GetS("ABC");
	
//	var reply = client.PostOrder("ABC", 100);
//	reply.Dump();
}

public class Order
{
    public string Symbol { get; set; }
    public decimal Price { get; set; }
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
        var requestUrl = $"http://{_url}:{_port}/api/orders";
        var response = GetRequest(requestUrl);
        var stocks = JsonConvert.DeserializeObject<HashSet<string>>(response);
        return stocks;
    }
	
	public void GetS(string symbol)
	{
		var requestUrl = $"http://{_url}:{_port}/api/orders/{symbol}";
        var response = GetRequest(requestUrl);
		response.Dump();
        var reply = JsonConvert.DeserializeObject<decimal>(response);
        reply.Dump();
	}
	
	public string PostOrder(string ticker, decimal price)
    {
        //replace with httpClient
        var requestUrl = $"http://{_url}:{_port}/api/orders";
        var request = (HttpWebRequest)WebRequest.Create(requestUrl);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        
		//var data = Encoding.ASCII.GetBytes($"symbol={ticker}:{price}");
		var order = new Order {Symbol = ticker, Price = price };
		var data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(order));
		request.ContentLength = data.Length;
		using (var stream = request.GetRequestStream())
		{
		    stream.Write(data, 0, data.Length);
		}

        var response = request.GetResponse() as HttpWebResponse;
        using (var stream = response.GetResponseStream())
        {
			response.StatusDescription.Dump();
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

// Define other methods and classes here
