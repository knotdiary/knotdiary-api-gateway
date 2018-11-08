using KnotDiary.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KnotDiary.Common.Web.Controllers
{
    [Produces("application/json")]
    public class BaseApiController : Controller
    {
        protected readonly ILogHelper _logHelper;
        protected readonly IConfigurationHelper _configurationHelper;

        public BaseApiController(ILogHelper logHelper, IConfigurationHelper configurationHelper)
        {
            _logHelper = logHelper;
            _configurationHelper = configurationHelper;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetErrorResponse(Exception exception, string message)
        {
            var controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            var actionName = ControllerContext.RouteData.Values["action"].ToString();
            _logHelper.LogError(exception, $"{controllerName}.{actionName} - Error calling {actionName} - {message}");

            return BadRequest(message);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetErrorResponse(string message)
        {
            var controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            var actionName = ControllerContext.RouteData.Values["action"].ToString();
            _logHelper.LogError($"{controllerName}.{actionName} - Error calling {actionName} - {message}");

            return BadRequest(message);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetSuccessResponse<T>(bool success, string message, T data)
        {
            return Ok(new BaseResponse<T>
            {
                IsSuccess = success,
                Message = message,
                Data = data
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetUploadFileExtension(string fileName)
        {
            var splitFileName = fileName.Split('.').ToList();
            return splitFileName.Last();
        }
    }
}