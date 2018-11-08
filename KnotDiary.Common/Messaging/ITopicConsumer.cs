using System;
using System.Threading.Tasks;

namespace KnotDiary.Common.Messaging
{
    public interface ITopicConsumer
    {
        void ConsumeExchange<T>(
            string exchange,
            string routingKey = null,
            string queueName = null,
            Func<MessageTopicPayload<T>, Task<TopicProcessResult>> OnConsume = null) where T : IMessagePayload;
    }
}
