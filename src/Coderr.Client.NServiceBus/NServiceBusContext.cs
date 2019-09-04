using System;
using System.Collections.Generic;
using Coderr.Client.Reporters;
using NServiceBus.Transport;
using NServiceBus.Unicast.Messages;

namespace Coderr.Client.NServiceBus
{
    public class NServiceBusContext : ErrorReporterContext
    {
        public NServiceBusContext(object reporter, Exception exception) : base(reporter, exception)
        {
        }

        public byte[] RawBody { get; set; }
        public IReadOnlyDictionary<string, string> MessageHeaders { get; set; }
        public string MessageId { get; set; }
        public string ReplyToAddress { get; set; }
        public object Body { get; set; }
        public MessageMetadata Metadata { get; set; }
        public Type MessageType { get; set; }
        public bool IsHandled { get; set; }
        public object HandlerInstance { get; set; }
        public Type HandlerType { get; set; }
    }
}