using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using vk_web_mock.Services;

namespace vk_web_mock
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
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
               {
                   options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                   options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                   options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
                   //    options.SerializerSettings.Converters.Add(new BooleanConverter());
                   options.SerializerSettings.Converters.Add(new StringEnumConverter());
               }
                );

            services.AddCors();

            services.AddSingleton(new PodStore());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }
            // else
            // {
            //     app.UseExceptionHandler("/Error");
            // }

            app.UseCors(options =>{
                options
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
    public class BooleanConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    {
                        var value = reader.Value.ToString();
                        if (value == "True")
                            return true;
                        if (value == "False")
                            return false;
                        throw new Exception($"Unable to convert value '{value}' to boolean");
                    }
                case JsonToken.Boolean:
                    return reader.Value;
                default:
                    throw new Exception($"Unhandled JsonTokenType {reader.TokenType}");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            bool boolValue = (bool)value;
            writer.WriteValue(boolValue ? "True" : "False");
        }
    }
}
