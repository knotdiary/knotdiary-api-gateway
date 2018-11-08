namespace KnotDiary.Common.Messaging
{
    public class MessageTopicPayload<T> where T : IMessagePayload
    {
        public string TopicName { get; set; }

        public string Exchange { get; set; }

        public T Payload { get; set; }
    }

    public interface IMessagePayload {}
}
