using System;
using Coderr.Client.Config;
using Coderr.Client.NServiceBus;
using Coderr.Client.NServiceBus.Behaviors;
using Coderr.Client.NServiceBus.ContextProviders;
using NServiceBus;

// ReSharper disable once CheckNamespace
namespace Coderr.Client
{
    public static class ErrConfigurationExtensions
    {
        public static void RegisterCoderr(this EndpointConfiguration nServiceBusConfig,
            CoderrConfiguration configuration)
        {
            var pipeline = nServiceBusConfig.Pipeline;
            pipeline.Register(
                new CoderrErrorHandling(configuration),
                "Reports exceptions to Coderr");
            pipeline.Register(
                new CoderrSerializationErrorHandler(configuration),
                "Reports serialization exceptions to Coderr");

            pipeline.Register(typeof(TrackSlowMessageHandlersBehavior), "Logs a warning if a handler take more than a specified time");


            configuration.ContextProviders.Add(new MessageProvider());
            configuration.ContextProviders.Add(new HeadersProvider());
            configuration.ContextProviders.Add(new HandlerProvider());
        }

        /// <summary>
        /// Report when handlers take a long time to process a message.
        /// </summary>
        /// <param name="instance">instance</param>
        /// <param name="maxExecutionTime">Max duration</param>
        public static void ReportSlowMessageHandlers(this CoderrConfiguration instance, TimeSpan maxExecutionTime)
        {
            CoderrSharedData.MaxExecutionTime = maxExecutionTime;
        }
    }
}