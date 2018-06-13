using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Venus.AI.Bot.Models
{
    public static class RestApiClient
    {
        private enum Authentication
        {
            None,
            Ldap
        }
        private static Authentication authentication;
        private static WebRequest webRequest;
        private static string baseUrl;

        public static void Сonfigure(string url)
        {
            baseUrl = url;
            authentication = Authentication.None;
        }
        public static void Сonfigure(string url, string login, string password)
        {
            baseUrl = url;
            authentication = Authentication.Ldap;
        }

        public static bool Connect()
        {
            HttpResponseMessage response;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUrl);
                response = httpClient.GetAsync(httpClient.BaseAddress).Result;
            }
            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }
        public static bool Connect(out string error)
        {
            HttpResponseMessage response;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUrl);
                try
                {
                    response = httpClient.GetAsync(httpClient.BaseAddress).Result;
                    error = string.Empty;
                    return true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    return false;
                }
            }
        }
        public static async Task<bool> ConnectAsync()
        {
            HttpResponseMessage response;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUrl);
                response = await httpClient.GetAsync(httpClient.BaseAddress);
            }
            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }

        public static string Post(string jsonString)
        {
            //jsonString = "\"" + jsonString + "\"";
            webRequest = WebRequest.Create(baseUrl);
            webRequest.ContentType = "application/json; charset=utf-8";
            webRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
            }
            var httpResponse = webRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                return result;
            }
        }
        public static async Task<string> PostAsync(string jsonString)
        {
            //jsonString = "'" + jsonString + "'";

            webRequest = WebRequest.Create(baseUrl);
            webRequest.ContentType = "application/json; charset=utf-8";
            webRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(await webRequest.GetRequestStreamAsync()))
            {
                await streamWriter.WriteAsync(jsonString);
                await streamWriter.FlushAsync();
            }
            var httpResponse = await webRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = await streamReader.ReadToEndAsync();
                return result;
            }
        }

        public static string Get()
        {
            webRequest = WebRequest.Create(baseUrl);
            var httpResponse = webRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                return result;
            }
        }
        public static string Get(string baseUrl)
        {
            webRequest = WebRequest.Create(baseUrl);
            var httpResponse = webRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                return result;
            }
        }
        public static async Task<string> GetAsync()
        {
            webRequest = WebRequest.Create(baseUrl);
            var httpResponse = await webRequest.GetResponseAsync();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = await streamReader.ReadToEndAsync();
                return result;
            }
        }
        public static async Task<string> GetAsync(string baseUrl)
        {
            webRequest = WebRequest.Create(baseUrl);
            var httpResponse = await webRequest.GetResponseAsync();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = await streamReader.ReadToEndAsync();
                return result;
            }
        }
    }
}
