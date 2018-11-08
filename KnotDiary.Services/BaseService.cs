using KnotDiary.Common;

namespace KnotDiary.Services
{
    public class BaseService : IBaseService
    {
        protected readonly ILogHelper _logHelper;
        protected readonly IConfigurationHelper _configurationHelper;
        protected readonly ICacheManager _cacheManager;

        public BaseService(ILogHelper logHelper, IConfigurationHelper configurationHelper, ICacheManager cacheManager)
        {
            _logHelper = logHelper;
            _configurationHelper = configurationHelper;
            _cacheManager = cacheManager;
        }
    }
}
