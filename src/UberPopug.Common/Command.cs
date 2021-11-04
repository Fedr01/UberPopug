namespace UberPopug.Common
{
    public abstract class Command
    {
        public Command(string topic)
        {
            Topic = topic;
        }

        public string Topic { get; }
    }
}