namespace KnotDiary.Common.Messaging
{
    public interface ITopicProducer
    {
        void SendToExchange<T>(string exchangeName, string routingKey, T message);
    }
}
