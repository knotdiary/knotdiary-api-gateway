using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Text;

namespace KnotDiary.Common.Messaging
{
    public class TopicProducer : ITopicProducer
    {
        private readonly ITopicConnection _topicConnection;
        private readonly ILogger _logger;
        private readonly IConfigurationHelper _configurationHelper;
        private readonly string DefaultExchangeType = "fanout";


        public TopicProducer(ITopicConnection topicConnection, ILogger logger, IConfigurationHelper configurationHelper)
        {
            _topicConnection = topicConnection;
            _logger = logger;
            _configurationHelper = configurationHelper;
        }

        public void SendToExchange<T>(string exchangeName, string routingKey, T message)
        {
            if (string.IsNullOrEmpty(exchangeName))
            {
                _logger.Error("KnotDiary.Common.Messaging.TopicProducer - Invalid exchange - Empty parameter");
                return;
            }

            try
            {
                using (var connection = _topicConnection.GetConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: DefaultExchangeType);

                    var json = JsonConvert.SerializeObject(message);
                    var messageBody = Encoding.UTF8.GetBytes(json);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    if (routingKey == null) routingKey = "";

                    channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: properties, body: messageBody);
                    _logger.Information($"KnotDiary.Common.Messaging.TopicProducer - Successfully published message - Body: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"KnotDiary.Common.Messaging.TopicProducer - Failed to process message - Body: {message}");
            }
        }
    }
}
