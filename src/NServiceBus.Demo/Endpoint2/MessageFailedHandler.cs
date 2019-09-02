using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.Demo.Messages;

namespace NServiceBus.Demo.Endpoint2
{
    class MessageFailedHandler: IHandleMessages<MessageFailed>
    {
        public Task Handle(MessageFailed message, IMessageHandlerContext context)
        {
            Console.WriteLine("Failed!!");
            return Task.CompletedTask;
        }
    }
}
