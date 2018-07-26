using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.AiServices;

namespace Venus.AI.WebApi.Models.Utils
{
    internal class RestApiClient
    {
        private string _baseUrl;

        public RestApiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        #region Ping
        public bool HostActive()
        {
            using(Ping p = new Ping())
            {
                string host = _baseUrl;
                bool result = false;
                try
                {
                    PingReply reply = p.Send(host, 2000);
                    if (reply.Status == IPStatus.Success)
                        return true;
                }
                catch { }
                return result;
            }
            
        }
        public async Task<bool> IsHostActiveAsync()
        {
            using (Ping p = new Ping())
            {
                string host = _baseUrl;
                bool result = false;
                try
                {
                    PingReply reply = await p.SendPingAsync(host, 2000);
                    if (reply.Status == IPStatus.Success)
                        return true;
                }
                catch { }
                return result;
            }

        }
        #endregion
        #region Get
        public string Get()
        {
            WebRequest webRequest = WebRequest.Create(_baseUrl);
            using (WebResponse resp = webRequest.GetResponse())
            using (Stream stream = resp.GetResponseStream())
            using (StreamReader sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
        public async Task<string> GetAsync()
        {
            WebRequest webRequest = WebRequest.Create(_baseUrl);

            using (WebResponse resp = webRequest.GetResponse())
            using (Stream stream = resp.GetResponseStream())
            using (StreamReader sr = new StreamReader(stream))
            {
                return await sr.ReadToEndAsync();
            }
        }
        #endregion
        #region Post
        public string Post(string jsonString)
        {
            WebRequest webRequest = WebRequest.Create(_baseUrl);
            webRequest.ContentType = "application/json; charset=utf-8";
            webRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
            }
            using (WebResponse resp = webRequest.GetResponse())
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }
        public async Task<string> PostAsync(string jsonString)
        {
            WebRequest webRequest = WebRequest.Create(_baseUrl);
            webRequest.ContentType = "application/json; charset=utf-8";
            webRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                await streamWriter.WriteAsync(jsonString);
                await streamWriter.FlushAsync();
            }
            using (WebResponse resp = webRequest.GetResponse())
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                return await sr.ReadToEndAsync();
            }
        }
        #endregion
    }
}
