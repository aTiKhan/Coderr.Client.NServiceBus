using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coderr.Client.Config;
using Coderr.Client.ContextCollections;
using NServiceBus.Pipeline;

namespace Coderr.Client.NServiceBus.Behaviors
{
    public class CoderrErrorHandling : Behavior<IIncomingLogicalMessageContext>
    {
        private DuplicateDetector _duplicateDetector;

        public CoderrErrorHandling(CoderrConfiguration configuration)
        {
            _duplicateDetector = DuplicateDetector.Instance;
        }

        public CoderrErrorHandling(CoderrConfiguration configuration, DuplicateDetector detector)
        {
            _duplicateDetector = detector;
        }



        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                await next()
                    .ConfigureAwait(false);

                sw.Stop();
            }
            catch (Exception ex)
            {
                if (!EnsureNotReported(context))
                    return;

                var ctx = new NServiceBusContext(this, ex)
                {
                    Body = context.Message.Instance,
                    Metadata = context.Message.Metadata,
                    MessageType = context.Message.MessageType,
                    IsHandled = context.MessageHandled,
                    MessageHeaders = context.MessageHeaders,
                    MessageId = context.MessageId,
                    ReplyToAddress = context.ReplyToAddress
                };

                ctx.AddHighlightedProperty("MessageHandler", "Type");
                ctx.AddHighlightedCollection("MessageBody");
                ctx.AddTag("nservicebus");

                Err.Report(ctx);
                throw;
            }
        }

        private bool EnsureNotReported(IIncomingLogicalMessageContext context)
        {
            if (!_duplicateDetector.Validate(context.MessageId))
                return false;

            if (!context.Extensions.TryGet(out CoderrSharedData data))
            {
                data = new CoderrSharedData();
                context.Extensions.Set(data);
                return true;
            }

            return false;
        }
    }
}