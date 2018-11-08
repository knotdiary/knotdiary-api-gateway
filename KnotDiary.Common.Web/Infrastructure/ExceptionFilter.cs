using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace KnotDiary.Common.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly bool _includeExceptionDetail;

        public ExceptionFilter(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _includeExceptionDetail = configuration.GetValue<bool>("Api:IncludeExceptionDetail");
        }

        public void OnException(ExceptionContext context)
        {
            _logger.Error(context.Exception, context.Exception.Message);

            var result = _includeExceptionDetail
                ? new JsonResult(new { context.Exception.Message, context.Exception })
                : new JsonResult(new { Message = "An unexpected error occurred." });

            result.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
