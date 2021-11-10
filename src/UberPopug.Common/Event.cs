namespace UberPopug.Common
{
    public abstract class Event
    {
        protected Event(string eventName)
        {
            EventName = eventName;
        }

        public string EventName { get; }
    }
}