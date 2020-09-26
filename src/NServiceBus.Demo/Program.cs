using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Client.Config;
using Coderr.Client.NServiceBus;
using NServiceBus.Demo.Messages;

namespace NServiceBus.Demo
{
    class Program
    {
        static void Main(string[] args)
        {

            var identity = new GenericIdentity("FakeUser");
            Thread.CurrentPrincipal = new GenericPrincipal(identity, new string[0]);

            ConfigureCoderr();

            Run().GetAwaiter().GetResult();

            Console.WriteLine("Press ENTER to quit");
            Console.ReadLine();
        }

        private static async Task Run()
        {
            var ep1 = await StartEndpoint("Ep1");
            var ep2 = await StartEndpoint("Ep2");


            var message = new FailingMessage();
            await ep1.Send("Ep1", message)
                .ConfigureAwait(false);

        }

        private static void ConfigureCoderr()
        {
            var url = new Uri("http://localhost:50473/");
            Err.Configuration.Credentials(url,
                "38dfcbc0f130481a9cdacf04742e1665",
                "ba13c02ddd124bdf8a42082ecfbfae89");

            Err.Configuration.ReportSlowMessageHandlers(TimeSpan.FromSeconds(1));
            Err.Configuration.AddPartition(AttachFacilityId);
        }

        private static void AttachFacilityId(PartitionContext x)
        {
            var context = x.ReporterContext as NServiceBusContext;
            if (context?.Body == null)
                return;

            var facilityIdProp = context.Body.GetType().GetProperty("FacilityId");
            if (facilityIdProp == null)
                return;

            var value = facilityIdProp.GetValue(context.Body);
            if (value != null)
                x.AddPartition("FacilityId", value.ToString());
        }

        private static async Task<IEndpointInstance> StartEndpoint(string name)
        {
            var endpointConfiguration = new EndpointConfiguration(name);
            endpointConfiguration.AutoSubscribe().DisableFor<MessageFailed>();
            endpointConfiguration.UsePersistence<LearningPersistence>();
            var t = endpointConfiguration.UseTransport<LearningTransport>();
            t.Routing().RouteToEndpoint(typeof(FailingMessage), "Ep1");
            t.Routing().RouteToEndpoint(typeof(MessageFailed), "Ep2");

            endpointConfiguration.Recoverability()
                .Immediate(x => x.NumberOfRetries(0))
                .Delayed(x => x.NumberOfRetries(0));

            endpointConfiguration.RegisterCoderr(Err.Configuration);

            return await Endpoint.Start(endpointConfiguration);
        }
    }
}
