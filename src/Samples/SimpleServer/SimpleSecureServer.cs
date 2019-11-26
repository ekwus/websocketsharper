using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharper;
using WebSocketSharper.Server;

namespace SimpleServer
{
    public class SimpleSecureServer : IHostedService
    {
        private readonly ILogger m_logger;
        private readonly IHostApplicationLifetime m_appLifetime;
        private WebSocketServer m_server;

        public SimpleSecureServer(ILogger<SimpleSecureServer> logger, IHostApplicationLifetime appLifetime)
        {
            m_logger = logger;
            m_appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            m_appLifetime.ApplicationStarted.Register(OnStarted);
            m_appLifetime.ApplicationStopping.Register(OnStopping);
            m_appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            m_logger.LogDebug("OnStarted Called");
            m_server = new WebSocketServer(m_logger, 8855, true);
            m_server.SslConfiguration.ServerCertificate = new X509Certificate2("cid.root.cert.pfx", "secretpassword");
            m_server.SslConfiguration.ClientCertificateRequired = true;
            m_server.SslConfiguration.ClientCertificateValidationCallback += (sender, cert, chain, errors) =>
            {
                return true;
            };

            m_server.AddWebSocketService<ChatBehaviour>("/chat");

            m_server.Start();

            Console.WriteLine("Press Ctrl+C to exit");
        }

        private void OnStopping()
        {
            m_logger.LogDebug("OnStopping Called");
            m_server.Stop();
        }

        private void OnStopped()
        {
            m_logger.LogDebug("OnStopped Called");
        }
    }

    public class ChatBehaviour : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var chars = e.Data.ToCharArray();
            Array.Reverse(chars);
            Send(new String(chars));
        }
    }
}
