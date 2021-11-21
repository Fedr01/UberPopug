using System;

namespace UberPopug.SchemaRegistry
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Uploading schemas to kafka!");
            SchemaRegistryUploader.Upload();
        }
    }
}
