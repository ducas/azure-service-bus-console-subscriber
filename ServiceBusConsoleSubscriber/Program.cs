using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServiceBusConsoleSubscriber
{

    public class Program
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Main(string[] args)
        {
            Configuration = BuildConfiguration();

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

            Run(cts.Token);
        }

        private static IConfiguration BuildConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return configBuilder.Build();
        }

        private static void Run(CancellationToken token)
        {
            Console.WriteLine("Starting...");

            var count = 0;
            var receiver = new SubscriptionClient(
                new ServiceBusConnectionStringBuilder(Configuration.GetConnectionString("ServiceBus")),
                Configuration["SubscriptionName"]
                );

            receiver.RegisterMessageHandler(
                (message, ct) =>
                {
                    count++;
                    var body = Encoding.UTF8.GetString(message.Body);
                    Console.WriteLine($"{count}: Received {message.MessageId}: {body}");
                    return Task.CompletedTask;
                },
                new MessageHandlerOptions((e) => { Console.WriteLine($"Error: {e.Exception.Message}"); return Task.CompletedTask; }) { AutoComplete = true, MaxConcurrentCalls = 100 });

            token.WaitHandle.WaitOne();

            Console.WriteLine($"Received {count} messages.");
            Console.WriteLine("Closed.");
        }
    }
}