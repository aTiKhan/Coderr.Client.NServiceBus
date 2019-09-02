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

            configuration.ContextProviders.Add(new BodyProvider());
            configuration.ContextProviders.Add(new HeadersProvider());
        }

        public static void TrackSlowMessages(this CoderrConfiguration instance, TimeSpan maxExecutionTime)
        {
            CoderrSharedData.MaxExecutionTime = maxExecutionTime;
        }
    }
}