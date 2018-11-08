using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Serilog;

namespace KnotDiary.Services.Http
{
    public class HttpClientWrapper : HttpClient
    {
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public HttpClientWrapper(ILogger logger, string baseAddress, int timeoutInMilliseconds, JsonSerializerSettings serializerSettings = null)
        {
            _logger = logger;
            
            BaseAddress = baseAddress.EndsWith("/")
                ? new Uri(baseAddress)
                : new Uri(baseAddress + "/");

            Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

            _jsonSerializerSettings = serializerSettings;
        }
        
        public async Task<T> GetAsync<T>(string path)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, path);
            return await GetAsync<T>(httpRequestMessage);
        }

        public async Task<T> GetAsync<T>(string path, IDictionary<string, string> headers)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, path);
            AddHeaders(httpRequestMessage, headers);
            return await GetAsync<T>(httpRequestMessage);
        }

        public async Task<string> GetAsHtmlAsync(string path, IDictionary<string, string> headers)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, path);
            AddHeaders(httpRequestMessage, headers);

            var httpResponseMessage = await SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var response = await httpResponseMessage.Content.ReadAsStringAsync();
                return response;
            }

            throw new HttpStatusException(
                httpResponseMessage.StatusCode,
                $"A call to {httpRequestMessage.Method} {httpRequestMessage.RequestUri} failed with status {(int)httpResponseMessage.StatusCode} ({httpResponseMessage.StatusCode}).");
        }

        private async Task<T> GetAsync<T>(HttpRequestMessage httpRequestMessage)
        {
            var httpResponseMessage = await SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var response = await httpResponseMessage.Content.ReadAsStringAsync();
                return _jsonSerializerSettings == null ?
                    JsonConvert.DeserializeObject<T>(response) :
                    JsonConvert.DeserializeObject<T>(response, _jsonSerializerSettings);
            }

            throw new HttpStatusException(
                httpResponseMessage.StatusCode,
                $"A call to {httpRequestMessage.Method} {httpRequestMessage.RequestUri} failed with status {(int)httpResponseMessage.StatusCode} ({httpResponseMessage.StatusCode}).");
        }
        
        public async Task<TS> PostAsync<TR, TS>(string path, TR requestObject, IDictionary<string, string> headers = null)
        {
            var contentJson = requestObject is string ?
                requestObject.ToString() :
                JsonConvert.SerializeObject(requestObject);
            
            var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new StringContent(contentJson, Encoding.UTF8) };
            if (headers != null)
            {
                AddHeaders(request, headers);
            }

            return await PostAsync<TS>(request, path);
        }

        public async Task<TS> PostAsync<TR, TS>(string path, TR requestObject, string contentType, IDictionary<string, string> headers = null)
        {
            var contentJson = requestObject is string ?
                requestObject.ToString() :
                JsonConvert.SerializeObject(requestObject);

            var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = string.IsNullOrEmpty(contentType) ?
                new StringContent(contentJson, Encoding.UTF8) : 
                new StringContent(contentJson, Encoding.UTF8, contentType);
            
            if (headers != null)
            {
                AddHeaders(request, headers);
            }

            return await PostAsync<TS>(request, path);
        }
        
        private async Task<TS> PostAsync<TS>(HttpRequestMessage request, string path)
        {
            string jsonString;
            HttpResponseMessage response;

            try
            {
                response = await SendAsync(request);
                jsonString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed Post call to relativeUri API. {uri}, {response}", path, ex);
                throw;
            }

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                _logger.Warning("Failed Post call to relativeUri API. {uri}, {response}, {jsonString}", path, response, jsonString);
            }

            if (response.IsSuccessStatusCode)
            {
                return _jsonSerializerSettings == null ?
                    JsonConvert.DeserializeObject<TS>(jsonString) :
                    JsonConvert.DeserializeObject<TS>(jsonString, _jsonSerializerSettings);
            }
            
            throw new HttpStatusException(
                response.StatusCode,
                $"A call to {request.Method} {request.RequestUri} failed with status {(int)response.StatusCode} ({response.StatusCode}).");
        }
        
        private static void AddHeaders(HttpRequestMessage request, IDictionary<string, string> headers)
        {
            if (headers == null) return;

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
    }
}
