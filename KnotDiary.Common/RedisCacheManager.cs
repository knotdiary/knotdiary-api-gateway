using ServiceStack.Redis;

namespace KnotDiary.Common
{
    public class RedisCacheManager : ICacheManager
    {
        private readonly IRedisClient _redisClient;

        public RedisCacheManager(IRedisClientsManager redisClientsManager)
        {
            _redisClient = redisClientsManager.GetClient();
        }

        public T GetCachedItemByKey<T>(string key)
        {
            var value = _redisClient.Get<T>(key);
            return value;
        }

        public void SetCache<T>(string key, T value)
        {
            _redisClient.Set(key, value);
        }

        public void RemoveCache(string key)
        {
            _redisClient.Remove(key);
        }
    }
}
