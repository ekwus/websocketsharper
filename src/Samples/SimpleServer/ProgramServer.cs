using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharper;
using WebSocketSharper.Server;
using Microsoft.Extensions.Logging;

namespace SimpleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WindowWidth = 180;
            Console.WindowHeight = 60;
            Console.Clear();

            Console.WriteLine("WebSocketSharper Simple Server");

            CreateHostBuilder(args).Build().Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureLogging(lb =>
                lb.AddConsole().SetMinimumLevel(LogLevel.Trace)
            )
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                //
                // Register our app
                //
                builder.RegisterType<SimpleSecureServer>().As<IHostedService>().InstancePerDependency();
            });
    }

}
