using RabbitMQ.Client;

namespace KnotDiary.Common.Messaging
{
    public interface ITopicConnection
    {
        IConnection GetConnection();
    }
}
