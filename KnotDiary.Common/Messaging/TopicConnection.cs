using RabbitMQ.Client;

namespace KnotDiary.Common.Messaging
{
    public class TopicConnection : ITopicConnection
    {
        private IConfigurationHelper _configurationHelper;

        public TopicConnection(IConfigurationHelper configurationHelper)
        {
            _configurationHelper = configurationHelper;
        }
        
        public IConnection GetConnection()
        {
            return GenerateConnection();
        }

        private IConnection GenerateConnection()
        {
            var factory = new ConnectionFactory()
            {
                UserName = _configurationHelper.GetAppSettings("Messaging:UserName"),
                Password = _configurationHelper.GetAppSettings("Messaging:Password"),
                HostName = _configurationHelper.GetAppSettings("Messaging:HostName"),
                Port = _configurationHelper.GetAppSettingsAsInt("Messaging:Port", 5672)
            };
            return factory.CreateConnection();
        }
    }
}
