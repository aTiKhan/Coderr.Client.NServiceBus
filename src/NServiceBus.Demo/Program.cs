using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Client.NServiceBus;
using NServiceBus.Demo.Messages;

namespace NServiceBus.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var identity = new GenericIdentity("FakeUser");
            Thread.CurrentPrincipal = new GenericPrincipal(identity, new string[0]);

            var url = new Uri("http://localhost:50473/");
            Err.Configuration.Credentials(url,
                "db47b658b788476eabd916534c039893",
                "93e6a190d9154ce88448581b73631d4a");

            var ep1 = await StartEp("Ep1");
            var ep2 = await StartEp("Ep2");


            var message = new FailingMessage();
            await ep1.Send("Ep1", message)
                .ConfigureAwait(false);

            Console.WriteLine("Press ENTER to quit");
            Console.ReadLine();
        }

        private static async Task<IEndpointInstance> StartEp(string name)
        {
            var endpointConfiguration = new EndpointConfiguration(name);
            endpointConfiguration.UsePersistence<LearningPersistence>();
            var t = endpointConfiguration.UseTransport<LearningTransport>();
            t.Routing().RouteToEndpoint(typeof(FailingMessage), "Ep1");
            t.Routing().RouteToEndpoint(typeof(MessageFailed), "Ep2");

            Err.Configuration.ReportSlowMessageHandlers(TimeSpan.FromSeconds(1));
            endpointConfiguration.RegisterCoderr(Err.Configuration);

            return await Endpoint.Start(endpointConfiguration);
        }
    }
}
