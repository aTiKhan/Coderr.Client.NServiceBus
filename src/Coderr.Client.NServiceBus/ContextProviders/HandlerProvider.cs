using System.Collections.Generic;
using System.Reflection;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;

namespace Coderr.Client.NServiceBus.ContextProviders
{
    internal class HandlerProvider : IContextCollectionProvider
    {
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (!(context is NServiceBusContext ctx))
                return null;

            var props = new Dictionary<string, string>();
            if (ctx.HandlerType != null)
                props.Add("Type", ctx.HandlerType.FullName);

            if (ctx.HandlerInstance != null)
            {
                var handlerProps = ctx.HandlerInstance.GetType()
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var fieldInfo in handlerProps)
                    props.Add(fieldInfo.Name, fieldInfo.GetValue(ctx.HandlerInstance)?.ToString() ?? "null");
            }

            return props.Count == 0 ? null : new ContextCollectionDTO(Name, props);
        }

        public string Name { get; } = "MessageHandler";
    }
}