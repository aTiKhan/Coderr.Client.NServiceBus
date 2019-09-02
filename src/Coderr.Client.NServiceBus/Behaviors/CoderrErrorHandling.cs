using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coderr.Client.Config;
using NServiceBus.Pipeline;

namespace Coderr.Client.NServiceBus.Behaviors
{
    public class CoderrErrorHandling : Behavior<IIncomingLogicalMessageContext>
    {
        public CoderrErrorHandling(CoderrConfiguration configuration)
        {
            
        }

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                await next()
                    .ConfigureAwait(false);

                sw.Stop();
                if (sw.Elapsed > CoderrSharedData.MaxExecutionTime)
                {
                    
                }
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

                Err.Report(ctx);
                throw;
            }
        }

        private static bool EnsureNotReported(IIncomingLogicalMessageContext context)
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