using Microsoft.AspNetCore.Authentication;
using MyServerAPI.Handlers;
using MyServerAPI.Services;

namespace MyServerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddGrpc();

            builder.Services.AddScoped<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "ApiKey";
            }).AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", configureOptions => { });

            builder.Services.AddAuthorization();


            var app = builder.Build();


            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapGrpcService<MessageInventoryService>();


            app.Run();
        }

    }
}
