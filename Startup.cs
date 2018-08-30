using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using vk_web_mock.Services;

namespace vk_web_mock
{
    public class Startup
    {
        private PodStore _podStore;
        private ILogger<Startup> _logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
               {
                   options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                   options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                   options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
                   options.SerializerSettings.Converters.Add(new StringEnumConverter());
               }
                );

            services.AddCors();

            services.AddSingleton(new PodStore());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(options =>{
                options
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });

            app.UseStaticFiles();

            _podStore = new PodStore();
            _logger = app.ApplicationServices.GetService<ILogger<Startup>>();

            app.HandlePath("/capacity", GetCapacity);
            app.HandlePath("/nodeConditions", GetNodeConditions);
            app.HandlePath("/nodeAddresses", GetNodeAddresses);
            app.HandlePath("/getPods", GetPods);
            app.HandlePath("/getPodStatus", GetPodStatus);
            app.HandlePath("/createPod", CreatePod);
            app.HandlePath("/updatePod", UpdatePod);
            app.HandlePath("/deletePod", DeletePod);
            app.HandlePath("/getContainerLogs", GetContainerLogs);

            app.UseMvc(); // handle Razor page for homepage

            app.Run(context =>
            {
                _logger.LogWarning($"Unmatched: {context.Request.Path}");
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Task.CompletedTask;
            });
        }
    }
}
