using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coderr.Client.ContextCollections;
using NServiceBus.Pipeline;

namespace Coderr.Client.NServiceBus.Behaviors
{
    public class TrackSlowMessageHandlersBehavior :
        Behavior<IInvokeHandlerContext>
    {
        private DuplicateDetector _duplicateDetector = DuplicateDetector.Instance;

        public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
        {
            var sw = Stopwatch.StartNew();
            await next();
            sw.Stop();
            if (sw.Elapsed > CoderrSharedData.MaxExecutionTime && _duplicateDetector.Validate(context.MessageId))
                ReportSlowMessageHandler(context, sw.Elapsed);
        }

        private void ReportSlowMessageHandler(IInvokeHandlerContext context, TimeSpan elapsed)
        {
            var ex = new SlowMessageHandlerException(context.MessageHandler.HandlerType.FullName, elapsed);
            var ctx = new NServiceBusContext(this, ex)
            {
                HandlerInstance = context.MessageHandler.Instance,
                HandlerType = context.MessageHandler.HandlerType,
                Body = context.MessageBeingHandled,
                Metadata = context.MessageMetadata,
                MessageType = context.MessageMetadata.MessageType,
                IsHandled = !context.HandlerInvocationAborted,
                MessageHeaders = context.MessageHeaders,
                MessageId = context.MessageId,
                ReplyToAddress = context.ReplyToAddress
            };

            ctx.AddHighlightedProperty("MessageHandler", "Type");
            ctx.AddHighlightedCollection("MessageBody");
            ctx.AddTag("performance");
            ctx.AddTag("nservicebus");

            Err.Report(ctx);
        }
    }
}