using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;

namespace Coderr.Client.NServiceBus.ContextProviders
{
    public class BodyProvider : IContextCollectionProvider
    {
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (!(context is NServiceBusContext ctx))
                return null;

            if (ctx.Body != null)
                return ctx.Body.ToContextCollection(Name);


            var body = ctx.RawBody?.Length < 50000 ? string.Join(", ", ctx.RawBody) : "[Too large message]";
            return new ContextCollectionDTO(Name, new Dictionary<string, string>() {{"Bytes", body}});

        }

        public string Name { get; private set; } = "MessageBody";
    }
}
