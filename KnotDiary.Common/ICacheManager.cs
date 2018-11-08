namespace KnotDiary.Common
{
    public interface ICacheManager
    {
        void SetCache<T>(string key, T value);
        T GetCachedItemByKey<T>(string key);
        void RemoveCache(string key);
    }
}
