using JamaCms.Common.Utils;
using JamaCms.Data.Library.Repository;
using Microsoft.Extensions.Logging;

namespace JamaCms.Common.Logger
{
    public class MongoDbLoggerProvider : ILoggerProvider
    {
        private readonly ILogRepository _logRepository;
        private readonly IConfigurationHelper _configurationHelper;

        public MongoDbLoggerProvider(ILogRepository logRepository, IConfigurationHelper configurationHelper)
        {
            _logRepository = logRepository;
            _configurationHelper = configurationHelper;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MongoDbLogger(categoryName, null, _logRepository, _configurationHelper);
        }

        public void Dispose()
        {
        }
    }
}
