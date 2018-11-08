using KnotDiary.ApiGateway.Models;
using System.Threading.Tasks;
using Serilog;
using KnotDiary.Services.Http;
using KnotDiary.Common.Web.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using KnotDiary.Models;

namespace KnotDiary.ApiGateway.Http
{
    public class HttpAuthService : IHttpAuthService
    {
        private readonly ILogger _logger;
        private readonly HttpClientWrapper _httpClient;
        private readonly HttpAuthServiceSettings _settings;

        public HttpAuthService(ILogger logger, HttpAuthServiceSettings httpSettings, JsonSerializerSettings serializerSettings)
        {
            _logger = logger;
            _settings = httpSettings;
            _httpClient = new HttpClientWrapper(_logger, httpSettings.BaseUrl, httpSettings.TimeoutInMilliseconds, serializerSettings);
        }

        public async Task<BaseResponse<User>> GetUser(string authToken)
        {
            _logger.Information($"HttpAuthService | GetUser | Calling UsersApi | token: {authToken}");
            var headers = new Dictionary<string, string> { { "Authorization", $"Bearer {authToken}" } };
            return await _httpClient.GetAsync<BaseResponse<User>>($"api/user", headers);
        }
    }
}
