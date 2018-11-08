using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace KnotDiary.Common.Messaging
{
    public class TopicConsumer : ITopicConsumer
    {
        private readonly ITopicConnection _topicConnection;
        private readonly ILogger _logger;
        private readonly IConfigurationHelper _configurationHelper;
        private readonly string DefaultExchangeType = "fanout";

        public TopicConsumer(ITopicConnection topicConnection, ILogger logger, IConfigurationHelper configurationHelper)
        {
            _topicConnection = topicConnection;
            _logger = logger;
            _configurationHelper = configurationHelper;
        }
        
        public void ConsumeExchange<T>(
            string exchange,
            string routingKey = null,
            string queueName = null, 
            Func<MessageTopicPayload<T>, Task<TopicProcessResult>> OnConsume = null) where T : IMessagePayload
        {
            if (string.IsNullOrEmpty(exchange))
            {
                _logger.Error("KnotDiary.Common.Messaging.TopicConsumer - Invalid exchange - Empty parameter");
                return;
            }

            var connection = _topicConnection.GetConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: exchange, type: DefaultExchangeType);

            if (routingKey == null)
            {
                routingKey = "";
            }

            if (string.IsNullOrEmpty(queueName))
            {
                queueName = channel.QueueDeclare().QueueName;
            }

            channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                _logger.Information($"KnotDiary.Common.Messaging.TopicConsumer - Received message - Body: {message}");

                if (OnConsume != null)
                {
                    var payload = new MessageTopicPayload<T>
                    {
                        TopicName = queueName,
                        Exchange = exchange,
                        Payload = JsonConvert.DeserializeObject<T>(message)
                    };

                    var processingResult = await OnConsume(payload).ConfigureAwait(false);

                    switch(processingResult)
                    {
                        case TopicProcessResult.SuccessAndAcknowledge:
                            _logger.Information($"KnotDiary.Common.Messaging.TopicConsumer - Successfully processed from callback - Tag {ea.DeliveryTag}");
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            break;
                        case TopicProcessResult.FailAndRetry:
                            _logger.Error($"KnotDiary.Common.Messaging.TopicConsumer - Failed processing from callback - Resending message - Tag {ea.DeliveryTag}");
                            channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                            break;
                        default:
                            _logger.Error($"KnotDiary.Common.Messaging.TopicConsumer - Failed processing from callback - Dump message - Tag {ea.DeliveryTag}");
                            channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                            break;
                    }
                }
                else
                {
                    _logger.Information($"KnotDiary.Common.Messaging.TopicConsumer - Successfully processed message - Tag {ea.DeliveryTag}");
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }
    }
}
