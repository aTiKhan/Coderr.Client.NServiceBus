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


            var ep1 = await StartEp("Ep1");
            var ep2 = await StartEp("Ep2");


            var message = new FailingMessage();
            await ep1.Send("Samples.UsernameHeader.Endpoint2", message)
                .ConfigureAwait(false);

            Console.WriteLine("Press ENTER to quit");
            Console.ReadLine();
        }

        private static async Task<IEndpointInstance> StartEp(string name)
        {
            var endpointConfiguration = new EndpointConfiguration(name);
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseTransport<LearningTransport>();

            endpointConfiguration.RegisterCoderr(Err.Configuration);

            return await Endpoint.Start(endpointConfiguration);
        }
    }
}
