using System;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharper;
using WebSocketSharper.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Autofac.Extensions.DependencyInjection;
using Autofac;

namespace SimpleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WindowWidth = 180;
            Console.WindowHeight = 60;
            Console.Clear();

            Console.WriteLine("WebSocketSharper SimpleClient");

            System.Threading.Thread.Sleep(3000);

            CreateHostBuilder(args).Build().Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureLogging(lb=>lb.AddConsole().SetMinimumLevel(LogLevel.Trace))
            .ConfigureContainer<ContainerBuilder>(builder =>
        {
            //
            // Register our app
            //
            builder.RegisterType<SimpleSecureClient>().As<IHostedService>().InstancePerDependency();
        });
    }
}
