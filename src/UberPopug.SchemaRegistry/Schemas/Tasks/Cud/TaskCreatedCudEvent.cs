using System;

namespace UberPopug.SchemaRegistry.Schemas.Tasks.Cud
{
    public class TaskCreatedCudEvent
    {
        public class V1 : Event
        {
            public Guid PublicId { get; set; }

            public string Description { get; set; }

            public V1() : base(nameof(TaskCreatedCudEvent), "v1")
            {
            }
        }

        public class V2 : Event
        {
            public Guid PublicId { get; set; }
            
            public string Title { get; set; }

            public string JiraId { get; set; }

            public V2() : base(nameof(TaskCreatedCudEvent), "v2")
            {
            }
        }
    }
}