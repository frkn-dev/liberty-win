using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LibertyApp.Models;

public class LocationFinder
{
    private const string GettingIpAddress = "http://checkip.dyndns.org";
    private const string GettingLocationAddress = "http://ip-api.com/json";
    private readonly HttpClient _client;
    
    public LocationFinder(HttpClient client) => _client = client;

    public async Task<string> FindLocation()
    {
        try
        {
            var ipResponse = await _client.GetStringAsync(GettingIpAddress);
            var ip = ipResponse.Split().Last();
            var locationResponse = await _client.GetStringAsync($"{GettingLocationAddress}/{ip}");
            var response = JObject.Parse(locationResponse);
            return response["country"].Value<string>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "Error while determine VPN location";
        }
    }
}