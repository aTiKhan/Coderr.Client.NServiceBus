using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.Demo.Messages;

namespace NServiceBus.Demo.Endpoint2
{
    class MessageFailedHandler: IHandleMessages<MessageFailed>
    {
        public async Task Handle(MessageFailed message, IMessageHandlerContext context)
        {
            throw new NotSupportedException("Incorrect handling");
        }
    }
}
