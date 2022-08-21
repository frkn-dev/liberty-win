using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LibertyApp.Models;

public class LocationFinder
{
    private readonly string _gettingIpAddress;
    private readonly string _gettingLocationAddress;
    private readonly HttpClient _client;
    
    public LocationFinder(string gettingLocationAddress, string gettingIpAddress, HttpClient client)
    {
        _gettingLocationAddress = gettingLocationAddress;
        _gettingIpAddress = gettingIpAddress;
        _client = client;
    }

    public async Task<string> FindLocation()
    {
        // TODO error handling
        try
        {
            var ipResponse = await _client.GetStringAsync(_gettingIpAddress);
            var ip = ipResponse.Split().Last();
            var locationResponse = await _client.GetStringAsync($"{_gettingLocationAddress}/{ip}");
            var response = JObject.Parse(locationResponse);
            return response["country"].Value<string>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "Error";
        }
    }
}