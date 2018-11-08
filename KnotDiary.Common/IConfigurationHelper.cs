using System.Collections.Generic;

namespace KnotDiary.Common
{
    public interface IConfigurationHelper
    {
        string GetAppSettings(string key);
        int GetAppSettingsAsInt(string key, int defaultValue);
        double GetAppSettingsAsDouble(string key, double defaultValue);
        IEnumerable<string> GetAppSettingsList(string key);
    }
}
