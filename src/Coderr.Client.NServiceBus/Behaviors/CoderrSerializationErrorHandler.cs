using System;
using System.Threading.Tasks;
using Coderr.Client.Config;
using Coderr.Client.ContextCollections;
using Coderr.Client.Processor;
using NServiceBus.Pipeline;

namespace Coderr.Client.NServiceBus.Behaviors
{
    public class CoderrSerializationErrorHandler : Behavior<ITransportReceiveContext>
    {
        private readonly ExceptionProcessor _processor;
        private DuplicateDetector _duplicateDetector;

        public CoderrSerializationErrorHandler(CoderrConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _processor = new ExceptionProcessor(configuration);
            _duplicateDetector = DuplicateDetector.Instance;
        }
        public CoderrSerializationErrorHandler(CoderrConfiguration configuration, DuplicateDetector duplicateDetector)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _processor = new ExceptionProcessor(configuration);
            _duplicateDetector = duplicateDetector;
        }

        public override async Task Invoke(ITransportReceiveContext context, Func<Task> next)
        {
            try
            {
                await next()
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!EnsureNotReported(context))
                    return;


                var ctx = new NServiceBusContext(this, ex)
                {
                    RawBody = context.Message.Body,
                    MessageHeaders = context.Message.Headers,
                    MessageId = context.Message.MessageId
                };

                ctx.AddHighlightedProperty("MessageHeaders", "MessageType");
                ctx.AddHighlightedCollection("MessageBody");
                ctx.AddTag("serialization");
                ctx.AddTag("nservicebus");

                _processor.Process(ctx);
                throw;
            }
        }

        private bool EnsureNotReported(ITransportReceiveContext context)
        {
            if (!_duplicateDetector.Validate(context.Message.MessageId))
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