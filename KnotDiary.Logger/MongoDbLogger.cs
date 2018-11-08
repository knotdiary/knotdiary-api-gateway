using System;
using JamaCms.Data.Library.Repository;
using JamaCms.Models.Entities;
using Microsoft.Extensions.Logging;
using JamaCms.Common.Utils;

namespace JamaCms.Common.Logger
{
    public class MongoDbLogger : ILogger
    {
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
        private ILogRepository _logRepository;
        private IConfigurationHelper _configurationHelper;

        public MongoDbLogger(string category, Func<string, LogLevel, bool> filter, ILogRepository logRepository, IConfigurationHelper configurationHelper)
        {
            _categoryName = category;
            _filter = filter;
            _logRepository = logRepository;
            _configurationHelper = configurationHelper;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter == null || _filter(_categoryName, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{ logLevel }: {message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            var applicationName = _configurationHelper.GetAppSettings("Application:Name");

            var log = new Log
            {
                Level = logLevel.ToString().ToLower(),
                EventId = eventId.Id,
                Message = message,
                Exception = exception?.InnerException?.Message,
                CreatedBy = applicationName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = applicationName,
                ModifiedDate = DateTime.UtcNow
            };

            _logRepository.Insert(log);
        }
        
        private class NoopDisposable : IDisposable
        {
            public static NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}
