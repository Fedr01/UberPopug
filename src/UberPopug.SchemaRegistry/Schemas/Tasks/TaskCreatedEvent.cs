using System;

namespace UberPopug.SchemaRegistry.Schemas.Tasks
{
    public class TaskCreatedEvent
    {
        public class V1 : Event
        {
            public Guid PublicId { get; set; }

            public string Description { get; set; }

            public V1() : base(nameof(TaskCreatedEvent), "v1")
            {
            }
        }

        public class V2 : Event
        {
            public Guid PublicId { get; set; }
            
            public string Title { get; set; }

            public string JiraId { get; set; }

            public V2() : base(nameof(TaskCreatedEvent), "v2")
            {
            }
        }
    }
}