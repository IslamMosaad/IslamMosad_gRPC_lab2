namespace ServerAPI.Services
{
    public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticationService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public bool Authenticate()
        {
            var context = _httpContextAccessor.HttpContext;

            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
                return false;

            var apiKeySettings = _configuration.GetSection("ApiKeys:client").Value;

            return apiKey == apiKeySettings;
        }
    }
}
