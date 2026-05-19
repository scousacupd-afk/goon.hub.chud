using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PlayFab;

namespace Blitz_Tag
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PlayFabSettings.staticSettings.TitleId = Constants.TitleId;
            PlayFabSettings.staticSettings.DeveloperSecretKey = Constants.SecretKey;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.Use(async (context, next) =>
            {
                var request = context.Request;

                try
                {
                    await next.Invoke();

                    Console.WriteLine($"{request.Method} {request.GetEncodedPathAndQuery()} => {context.Response.StatusCode}");
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] {request.Method} {request.GetEncodedPathAndQuery()} => {ex.Message}");
                    throw;
                }
            });

            app.UseWebSockets();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
