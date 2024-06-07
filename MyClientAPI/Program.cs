
using MyClientAPI.Services;
using static GRPS_Test.Protos.InventoryService;


namespace MyClientAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<IApiKeyProviderService, ApiKeyProviderService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddGrpcClient<InventoryServiceClient>(o =>
            {
                o.Address = new Uri("https://localhost:7153");
            }).AddCallCredentials((context, metadata, serviceProvider) =>
            {
                var apiKeyProvider = serviceProvider.GetRequiredService<IApiKeyProviderService>();
                var apiKey = apiKeyProvider.GetApiKey();
                metadata.Add("X-Api-Key", apiKey);
                return Task.CompletedTask;
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

    }
}
