using System.Collections.Generic;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;

namespace Coderr.Client.NServiceBus.ContextProviders
{
    public class MessageProvider : IContextCollectionProvider
    {
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (!(context is NServiceBusContext ctx))
                return null;

            if (ctx.Body != null)
            {
                var converter = new ObjectToContextCollectionConverter();
                var properties = converter.ConvertToDictionary("Body", ctx.Body);
                properties.Add("MessageType", ctx.Body.GetType().FullName);
                return new ContextCollectionDTO(Name, properties);
            }

            var body = ctx.RawBody?.Length < 50000 ? string.Join(", ", ctx.RawBody) : "[Too large message]";
            var props = new Dictionary<string, string> {{"Bytes", body}};
            if (ctx.MessageType != null)
                props.Add("MessageType", ctx.MessageType.FullName);
            return new ContextCollectionDTO(Name, props);
        }

        public string Name { get; } = "MessageBody";
    }
}