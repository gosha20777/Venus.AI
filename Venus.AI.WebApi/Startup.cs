using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Venus.AI.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            //app configure
            var config  = JsonConvert.DeserializeObject<Models.AppConfig>(File.ReadAllText("appconfig.json"));
            string msg = "Settings:\n\t" +
                $"ApiAiKey           : {Models.AppConfig.ApiAiKey}\n\t" +
                $"GoogleSpeechApiKey : {Models.AppConfig.GoogleSpeechApiKey}\n\t" +
                $"YandexSpeechApiKey : {Models.AppConfig.YandexSpeechApiKey}\n\t" +
                $"YandexTranslatorKey: {Models.AppConfig.YandexTranslatorKey}\n\t" +
                $"RnnTalkServiceUrlEn: {Models.AppConfig.RnnTalkServiceUrl}\n\t" +
                $"RnnTalkServiceUrlRu: {Models.AppConfig.RnnTalkServiceUrlRu}\n";
            Models.Utils.Log.Initialize();
            Models.Utils.Log.LogInformation(-1, 0, this.GetType().ToString(), msg);
        }
    }
}
