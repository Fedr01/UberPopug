namespace UberPopug.Common.Outbox
{
    public class OutboxLetter
    {
        private OutboxLetter()
        {
        }

        public OutboxLetter(string topic, string json)
        {
            Topic = topic;
            Body = json;
        }

        public int Id { get; private set; }
        public string Topic { get; private set; }
        public string Body { get; private set; }
    }
}