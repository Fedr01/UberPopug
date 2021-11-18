using System;

namespace UberPopug.SchemaRegistry.Schemas.Tasks.Cud
{
    public class TaskCreatedCudEvent : IEvent
    {
        public TaskCreatedCudEvent(Guid publicId, string title, string jiraId)
        {
            PublicId = publicId;
            Title = title;
            JiraId = jiraId;
        }

        public Guid PublicId { get; private set; }
        public string Title { get; private set; }
        public string JiraId { get; private set; }

        public EventMetaData MetaData { get; set; } = new EventMetaData(nameof(TaskCreatedCudEvent), "v1");
    }
}