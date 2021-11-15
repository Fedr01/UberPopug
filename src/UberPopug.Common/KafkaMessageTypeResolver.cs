using System;
using KafkaFlow;

namespace UberPopug.Common
{
    public class KafkaMessageTypeResolver : IMessageTypeResolver
    {
        private const string MessageType = "MessageType";

        public Type OnConsume(IMessageContext context)
        {
            var typeName = context.Headers.GetString(MessageType);

            return Type.GetType(typeName);
        }

        public void OnProduce(IMessageContext context)
        {
            context.Headers.SetString(
                MessageType,
                $"{context.Message.Value.GetType().FullName}, {context.Message.Value.GetType().Assembly.GetName().Name}");
        }
    }
}