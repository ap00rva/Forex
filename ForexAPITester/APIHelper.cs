using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public static class APIHelper
    {

        public static bool LoginToAccount(string Url, string apiKey, Login login, out string xToken, out string cstToken, out AccountMain acct)
        {
            bool retVal = false;
            xToken = string.Empty;
            cstToken = string.Empty;
            acct = new AccountMain();
            var client = new RestClient();
            client.BaseUrl = new Uri(Url);
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-IG-API-KEY", apiKey);
            request.AddHeader("Version", "2");
            request.AddJsonBody(login);

            var response = client.Execute(request);

            var responseHeaders = response.Headers;
            var responseBody = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                cstToken = responseHeaders.First(c => c.Name == "CST")?.Value.ToString() ?? String.Empty;
                xToken = responseHeaders.First(c => c.Name == "X-SECURITY-TOKEN")?.Value.ToString() ?? String.Empty;
                acct = JsonConvert.DeserializeObject<AccountMain>(responseBody);
                retVal = true;
            }
            else
            {
                retVal = false;
            }
            return retVal;

        }

        public static IRestResponse makeRequest(string Url, string apiKey, string XToken, string CstToken, Method method, string Version = "3")
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(Url);
            var request = new RestRequest();
            request.Method = method;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-IG-API-KEY", apiKey);

            request.AddHeader("X-SECURITY-TOKEN", XToken);
            request.AddHeader("CST", CstToken);

            request.AddHeader("Version", Version);
            var response = client.Execute(request);
            return response;

        }
        public static IRestResponse MakeTradeRequest(string Url, string apiKey, string XToken, string CstToken, TradeRequest postData)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(Url);
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-IG-API-KEY", apiKey);
            request.AddHeader("X-SECURITY-TOKEN", XToken);
            request.AddHeader("CST", CstToken);
            request.AddHeader("Version", "2");
            request.AddJsonBody(postData);
            var response = client.Execute(request);
            return response;

        }
        public static IRestResponse CloseTradeRequest(string Url, string apiKey, string XToken, string CstToken, CloseTradeRequest postData)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(Url);
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-IG-API-KEY", apiKey);
            request.AddHeader("X-SECURITY-TOKEN", XToken);
            request.AddHeader("CST", CstToken);
            request.AddHeader("Version", "1");
            request.AddHeader("_method", "DELETE");
            request.AddJsonBody(postData);
            var response = client.Execute(request);
            return response;

        }

        public static IRestResponse Positions(string Url, string apiKey, string XToken, string CstToken)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(Url);
            var request = new RestRequest();
            request.Method = Method.GET;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-IG-API-KEY", apiKey);
            request.AddHeader("X-SECURITY-TOKEN", XToken);
            request.AddHeader("CST", CstToken);
            request.AddHeader("Version", "2");
            var response = client.Execute(request);
            return response;

        }

    }
}
