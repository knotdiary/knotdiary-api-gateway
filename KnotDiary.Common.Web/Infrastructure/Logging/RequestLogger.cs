using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KnotDiary.Common.Web.Infrastructure.Logging
{
    public class RequestLogger
    {
        private const string MessageTemplate = "{Method} {Status} {TimeTaken}";

        private readonly ILogger<RequestLogger> _logger;

        readonly RequestDelegate _next;

        public RequestLogger(RequestDelegate next, ILogger<RequestLogger> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _next.Invoke(httpContext);
                stopwatch.Stop();

                var statusCode = httpContext.Response?.StatusCode;

                if (statusCode >= 500)
                {
                    _logger.LogError(MessageTemplate, httpContext.Request?.Method, statusCode, stopwatch.Elapsed.TotalMilliseconds);
                }
                else
                {
                    _logger.LogInformation(MessageTemplate, httpContext.Request?.Method, statusCode, stopwatch.Elapsed.TotalMilliseconds);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, MessageTemplate, httpContext.Request?.Method, 500, stopwatch.Elapsed.TotalMilliseconds);
                throw;
            }
        }
    }
}
