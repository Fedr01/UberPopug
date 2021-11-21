using Newtonsoft.Json;

namespace UberPopug.SchemaRegistry
{
    public class RegisterSchemaRequest
    {
        [JsonProperty("schema")] 
        public string Schema { get; set; }

        [JsonProperty("schemaType")]
        public string SchemaType { get; set; } = "JSON";
    }
}