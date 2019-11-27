using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharper;

namespace SimpleClient
{
    public class SimpleSecureClient : IHostedService
    {
        private readonly ILogger m_logger;
        private readonly IHostApplicationLifetime m_appLifetime;
        private WebSocket m_client;
        private bool m_running;

        public SimpleSecureClient(ILogger<SimpleSecureClient> logger, IHostApplicationLifetime appLifetime)
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
            m_running = true;

            m_client = new WebSocket(m_logger, "wss://localhost:8855/chat");
            m_client.SslConfiguration.ClientCertificates = new X509CertificateCollection();
            var localCert = new X509Certificate2("client.pfx", "password");
            m_client.SslConfiguration.ClientCertificates.Add(localCert);
            m_client.SslConfiguration.ClientCertificateSelectionCallback += (sender, host, certs, remote, issuers) =>
            {
                return localCert;
            };
            m_client.SslConfiguration.ServerCertificateValidationCallback += delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                // Validate this is our server
                return certificate.GetCertHashString() == "9C39BEDE53BC34F11609E6E8F8E56C0028E07213";
            };
            m_client.OnMessage += (sender, e) =>
            {
                m_logger.LogDebug(e.Data);
            };

            m_client.OnOpen += (sender, e) =>
            {
                m_client.Send("Hello");
            };

            m_client.Connect();

            Task.Run(() =>
            {
                string msg = null;
                while (m_running && !string.IsNullOrEmpty(msg = Console.ReadLine()))
                {
                    if (!m_client.IsAlive)
                    {
                        m_logger.LogDebug("Connection to the server has been closed");
                        break;
                    }
                    m_client.Send(msg);
                }
            });
        }

        private void OnStopping()
        {
            m_logger.LogDebug("OnStopping Called");
            m_running = false;
            m_client.Close();
        }

        private void OnStopped()
        {
            m_logger.LogDebug("OnStopped Called");
        }
    }
}
