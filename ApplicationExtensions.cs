using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace vk_web_mock
{
    public static class ApplicationExtensions
    {
        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-ddTHH:mm:ssZ",
            Converters = {
                    new StringEnumConverter()
                },
        };
        public static IApplicationBuilder HandlePath(this IApplicationBuilder app, PathString pathMatch, RequestDelegate handler)
        {
            return app.Map(pathMatch, a =>
            {
                a.Run(handler);
            });
        }
        public static async Task WriteJsonResponseAsync<T>(this HttpContext context, T data)
        {
            var jsonString = JsonConvert.SerializeObject(data, _serializerSettings);
            await context.Response.WriteAsync(jsonString);
        }

        public static string GetQueryStringValue(this HttpContext context, string name)
        {
            return context.Request.Query[name].FirstOrDefault();
        }

        // Return true if the request should be processed
        // Sets 404 Not Found StatusCode if not matching
        public static bool ShouldProcessFor(this HttpContext context, string method)
        {
            if (context.Request.Method != HttpMethods.Get)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }
            return true;
        }

        public static async Task<T> GetJsonBody<T>(this HttpContext context)
        {
            using (var reader = new StreamReader(context.Request.Body))
            {
                var content = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}