namespace UberPopug.SchemaRegistry.Schemas
{
    public interface IEvent
    {
        EventMetaData MetaData { get; set; }
    }

    public class Event : IEvent
    {
        public EventMetaData MetaData { get; set; }
    }
}