using Hydrogen.Integration.Cinsay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace Hydrogen.Integration.Cinsay
{
    public interface ICinsayClient
    {
        IEnumerable<CinsayPlayer> GetClientPlayers(string clientId);
    }

    public class CinsayClient : ICinsayClient
    {
        readonly string _appKey;
        readonly string _apiKey;

        public CinsayClient(string url, string appKey, string apiKey)
        {
            _apiKey = apiKey;
            _appKey = appKey;

             _client = new Lazy<HttpClient>(() =>
             {
                 var httpClient = new HttpClient();

                 httpClient.BaseAddress = new Uri(url);

                 //httpClient.Add("Content-Type", "application/json");
                 httpClient.DefaultRequestHeaders.Add("appKey", _appKey);
                 httpClient.DefaultRequestHeaders.Add("apiKey", _apiKey);
                 httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                 return httpClient;
             });
        }

        Lazy<HttpClient> _client;

        public IEnumerable<CinsayPlayer> GetClientPlayers(string clientId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/players/");

            request.Headers.Add("clientGuid", clientId);

            var result = _client.Value.SendAsync(request)
                .GetAwaiter().GetResult();

            var response = result.Content.ReadAsStringAsync()
                .GetAwaiter().GetResult();

            return JsonConvert.DeserializeObject<CinsayResponse<List<CinsayPlayer>>>(response).ResponseObject;         
        }
    }
}
