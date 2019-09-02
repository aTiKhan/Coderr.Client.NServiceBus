using System;
using System.Threading.Tasks;
using Coderr.Client.Config;
using Coderr.Client.Processor;
using NServiceBus.Pipeline;

namespace Coderr.Client.NServiceBus.Behaviors
{
    public class CoderrSerializationErrorHandler : Behavior<ITransportReceiveContext>
    {
        private readonly ExceptionProcessor _processor;

        public CoderrSerializationErrorHandler(CoderrConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _processor = new ExceptionProcessor(configuration);
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

                _processor.Process(ctx);
                throw;
            }
        }

        private static bool EnsureNotReported(ITransportReceiveContext context)
        {
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