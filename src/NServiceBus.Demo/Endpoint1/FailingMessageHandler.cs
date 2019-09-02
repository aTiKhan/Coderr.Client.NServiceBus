using System;
using System.Threading.Tasks;
using NServiceBus.Demo.Messages;

namespace NServiceBus.Demo.Endpoint1
{
    class FailingMessageHandler : IHandleMessages<FailingMessage>
    {
        public Task Handle(FailingMessage message, IMessageHandlerContext context)
        {
            throw new NotSupportedException("Incorrect handling");
        }
    }
}
