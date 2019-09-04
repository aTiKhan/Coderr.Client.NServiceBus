using System;

namespace Coderr.Client.NServiceBus.Behaviors
{
    internal class SlowMessageHandlerException : Exception
    {
        public SlowMessageHandlerException(string handlerTypeName, TimeSpan duration)
            : base($"Slow message handler '{handlerTypeName}': {duration.TotalMilliseconds}ms")
        {
        }
    }
}