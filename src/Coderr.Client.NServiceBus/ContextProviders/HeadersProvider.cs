using System.Collections.Generic;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;
using NServiceBus;

namespace Coderr.Client.NServiceBus.ContextProviders
{
    public class HeadersProvider : IContextCollectionProvider
    {
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (!(context is NServiceBusContext ctx))
                return null;

            if (ctx.MessageHeaders == null)
                return null;

            var props = new Dictionary<string, string>();
            foreach (var pair in ctx.MessageHeaders)
            {
                props[pair.Key] = pair.Value;
            }

            var messageType = ctx.MessageType?.FullName;
            if (messageType == null)
            {
                if (ctx.MessageHeaders.TryGetValue(Headers.EnclosedMessageTypes, out var value))
                {
                    var pos = value.IndexOf(',');
                    messageType = pos == -1 ? value : value.Substring(0, pos);
                }
            }

            props["MessageId"] = ctx.MessageId;
            props["ReplyToAddress"] = ctx.ReplyToAddress;
            props["IsHandled"] = ctx.IsHandled.ToString();
            if (messageType != null)
                props["MessageType"] = messageType;

            return new ContextCollectionDTO(Name, props);
        }

        public string Name { get; private set; } = "MessageHeaders";

    }
}
