using System;
using System.Collections.Generic;
using System.Text;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;

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

            props["MessageId"] = ctx.MessageId;
            props["MessageType"] = ctx.MessageType.ToString();
            props["ReplyToAddress"] = ctx.ReplyToAddress;
            props["IsHandled"] = ctx.IsHandled.ToString();
            
            return new ContextCollectionDTO(Name, props);
        }

        public string Name { get; private set; } = "MessageHeaders";

    }
}
