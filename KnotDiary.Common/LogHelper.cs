using Serilog;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace KnotDiary.Common
{
    public class LogHelper : ILogHelper
    {
        private readonly ILogger _logger;

        public LogHelper(ILogger logger)
        {
            _logger = logger;
        }

        public void LogError(string message)
        {
            var errorMessage = GenerateMessage(message);
            _logger.Error(errorMessage);
        }

        public void LogError(Exception exception, string message)
        {
            var errorMessage = GenerateMessage(message);
            _logger.Error(exception, errorMessage);
        }

        public void LogInfo(string message)
        {
            var msg = GenerateMessage(message);
            _logger.Information(msg);
        }

        private string GenerateMessage(string message)
        {
            var methodInfo = MethodBase.GetCurrentMethod();
            var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;

            var stackTrace = new StackTrace();
            var sf = stackTrace.GetFrame(0);
            var currentMethodName = sf.GetMethod();

            var completeMessage = new StringBuilder($"{fullName} | {currentMethodName}");
            if (!string.IsNullOrEmpty(message))
            {
                completeMessage.Append($"| {message}");
            }

            return completeMessage.ToString();
        }
    }
}
