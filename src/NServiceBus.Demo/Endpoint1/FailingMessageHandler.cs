using System;
using System.Threading.Tasks;
using NServiceBus.Demo.Messages;

namespace NServiceBus.Demo.Endpoint1
{
    class FailingMessageHandler : IHandleMessages<FailingMessage>
    {
        public async Task Handle(FailingMessage message, IMessageHandlerContext context)
        {
            await Task.Delay(1232);
            await context.Publish(new MessageFailed());
        }
    }
}
