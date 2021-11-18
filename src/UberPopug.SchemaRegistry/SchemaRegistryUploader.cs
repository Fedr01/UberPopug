using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using NJsonSchema.Generation;
using UberPopug.SchemaRegistry.Schemas;
using UberPopug.SchemaRegistry.Schemas.Users;

namespace UberPopug.SchemaRegistry
{
    public static class SchemaRegistryUploader
    {
        public static void Upload()
        {
            var settings = new JsonSchemaGeneratorSettings
            {
                FlattenInheritanceHierarchy = true
            };
            var generator = new JsonSchemaGenerator(settings);

            var events = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IEvent).IsAssignableFrom(t));

            var httpClient = new HttpClient();

            foreach (var ev in events)
            {
                var schema = generator.Generate(ev);
                var json = schema.ToJson();
                var job = JsonConvert.DeserializeObject(json);
                json = JsonConvert.SerializeObject(job);

                var request = new RegisterSchemaRequest
                {
                    Schema = json
                };

                var subject = $"{ev.Name}";

                var response = httpClient.PostAsync(
                    $"http://localhost:8081/subjects/{subject}/versions",
                    new StringContent(JsonConvert.SerializeObject(request),
                        Encoding.UTF8,
                        "application/vnd.schemaregistry+json")).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Failed to upload schema {subject}");
                }

                var result = response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}