using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceBus.Demo.Messages
{
    class MessageFailed : IEvent
    {
        public TimeSpan Elapsed { get; set; } = TimeSpan.FromMilliseconds(502828);
        public string OriginatingId { get; set; } = "ABC123";
    }
}
