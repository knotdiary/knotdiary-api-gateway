using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace KnotDiary.Common
{
    public class ConfigurationHelperJson : IConfigurationHelper
    {
        private IConfiguration Configuration { get; }

        public ConfigurationHelperJson(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string GetAppSettings(string key)
        {
            return Configuration[key];
        }

        public IEnumerable<string> GetAppSettingsList(string key)
        {
            var config = Configuration.GetSection(key).GetChildren();
            var list = new List<string>();

            foreach (var item in config)
            {
                list.Add(item.Value);
            }

            return list;
        }

        public int GetAppSettingsAsInt(string key, int defaultValue)
        {
            if (int.TryParse(GetAppSettings(key), out int value))
            {
                return value;
            }

            return defaultValue;
        }

        public double GetAppSettingsAsDouble(string key, double defaultValue)
        {
            if (double.TryParse(GetAppSettings(key), out double value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
