using Microsoft.Extensions.Logging;

namespace JamaCms.Common.Logger
{
    public static class MongoDbLoggerFactoryExtensions
    {
        public static ILoggerFactory AddMongoDb(this ILoggerFactory factory, ILoggerProvider provider)
        {
            factory.AddProvider(provider);
            return factory;
        }
    }
}
