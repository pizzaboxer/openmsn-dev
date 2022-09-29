using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenMSN.Data;
using OpenMSN.MSNPServer.Services;

using System.Net;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => services.AddDbContext<ApplicationDbContext>())
    .Build();

await host.RunAsync();

namespace OpenMSN.MSNPServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("[Main] Initializing services");

            var server = new NotificationServer(IPAddress.Any, 1863);
            server.Start();

            Console.WriteLine("[Main] Services initialized");

            for (; ; )
            {
                string? line = Console.ReadLine();

                // TODO: stats on newline?

                if (String.IsNullOrEmpty(line))
                    break;
            }

            server.Stop();

        }
    }
}