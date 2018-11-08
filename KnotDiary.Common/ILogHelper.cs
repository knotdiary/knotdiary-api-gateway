using System;

namespace KnotDiary.Common
{
    public interface ILogHelper
    {
        void LogError(string message);

        void LogError(Exception exception, string message);

        void LogInfo(string message);
    }
}
