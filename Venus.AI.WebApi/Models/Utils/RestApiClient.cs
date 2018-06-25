using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Utils
{
    internal static class RestApiClient
    {
        private static WebRequest _webRequest;
        private static string _baseUrl;

        public static void Сonfigure(string url)
        {
            _baseUrl = url;
        }

        public static bool Connect()
        {
            HttpResponseMessage response;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_baseUrl);
                response = httpClient.GetAsync(httpClient.BaseAddress).Result;
            }
            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }
        public static async Task<bool> ConnectAsync()
        {
            HttpResponseMessage response;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_baseUrl);
                response = await httpClient.GetAsync(httpClient.BaseAddress);
            }
            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }

        public static string Post(string jsonString)
        {
            _webRequest = WebRequest.Create(_baseUrl);
            _webRequest.ContentType = "application/json; charset=utf-8";
            _webRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(_webRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
            }
            var httpResponse = _webRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                return result;
            }
        }
        public static async Task<string> PostAsync(string jsonString)
        {
            _webRequest = WebRequest.Create(_baseUrl);
            _webRequest.ContentType = "application/json; charset=utf-8";
            _webRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(await _webRequest.GetRequestStreamAsync()))
            {
                await streamWriter.WriteAsync(jsonString);
                await streamWriter.FlushAsync();
            }
            var httpResponse = await _webRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = await streamReader.ReadToEndAsync();
                return result;
            }
        }

        public static string Get()
        {
            _webRequest = WebRequest.Create(_baseUrl);
            var httpResponse = _webRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                return result;
            }
        }
        public static async Task<string> GetAsync()
        {
            _webRequest = WebRequest.Create(_baseUrl);
            var httpResponse = await _webRequest.GetResponseAsync();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = await streamReader.ReadToEndAsync();
                return result;
            }
        }
    }
}
