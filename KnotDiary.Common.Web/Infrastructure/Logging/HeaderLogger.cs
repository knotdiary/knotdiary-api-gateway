using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace KnotDiary.Common.Web.Infrastructure.Logging
{
    public class HeaderLogger
    {
        readonly RequestDelegate _next;

        public HeaderLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.Headers.TryGetValue(Headers.System, out StringValues systemId);
            context.Request.Headers.TryGetValue(Headers.Correlation, out StringValues correlationId);

            using (LogContext.PushProperty(Headers.System, systemId.FirstOrDefault()))
            using (LogContext.PushProperty(Headers.Correlation, correlationId.FirstOrDefault()))
            {
                await _next.Invoke(context);
            }
        }
    }
}
