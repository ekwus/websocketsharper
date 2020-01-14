using System;
using System.Threading;
using TestSupport;
using WebSocketSharper;
using WebSocketSharper.Server;
using Xunit;
using Xunit.Abstractions;

namespace WebSocketSharpererTests
{
    public class WebSocketTests : BaseTest
    {
        public WebSocketTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {

        }

        public enum CheckPoint
        {
            ClientReceived,
            ClientConnectionClosed
        }

        [Fact]
        public void TestConnect()
        {
            var gate = new TestGate<CheckPoint>();
            var wss = new WebSocketServer(LOG, 8844);
            wss.AddWebSocketService<WebSocketTestBehaviour>("/test");
            wss.Start();


            using (var wsc = new WebSocket(LOG, "ws://localhost:8844/test", true))
            {
                wsc.OnMessage += (sender, e) =>
                {
                    gate.Set(CheckPoint.ClientReceived);
                };

                wsc.Connect();

                wsc.Send("BALUS");

                gate.AssertWaitFor(CheckPoint.ClientReceived, 5000);
            }


            wss.Stop();
        }


        [Fact]
        public void TestWithObservableMessages()
        {
            var gate = new TestGate<CheckPoint>();
            var wss = new WebSocketServer(LOG, 8844);
            wss.AddWebSocketService<WebSocketTestBehaviour>("/test");
            wss.Start();


            using (var wsc = new WebSocket(LOG, "ws://localhost:8844/test", true))
            {
                wsc.MessageReceived.Subscribe((m) =>
                {
                    gate.Set(CheckPoint.ClientReceived);
                });

                wsc.Connect();

                wsc.Send("Check Observable route works!");

                gate.AssertWaitFor(CheckPoint.ClientReceived, 5000);
            }


            wss.Stop();
        }

        [Fact]
        public void TestConnectClientFirst()
        {
            var gate = new TestGate<CheckPoint>();


            // Create client
            int clientMessageCount = 0;
            int clientCloseCount = 0;
            int clientRetryCount = 0;
            var wsc = new WebSocket(LOG, "ws://localhost:8844/test", true);
            wsc.ReconnectDelay = TimeSpan.FromSeconds(3);
            wsc.OnMessage += (sender, e) =>
            {
                clientMessageCount++;
                gate.Set(CheckPoint.ClientReceived);
            };

            wsc.OnError += (sender, e) =>
            {
            };

            wsc.OnOpen += (sender, e) =>
            {

            };

            wsc.OnClose += (sender, e) =>
            {
                var code = (CloseStatusCode)e.Code;

                if (
                    (code == CloseStatusCode.Normal) ||
                    (code == CloseStatusCode.NoStatus)
                    )
                {
                    clientCloseCount++;
                    gate.Set(CheckPoint.ClientConnectionClosed);
                }
                else
                {
                    clientRetryCount++;
                }
            };

            wsc.ConnectTaskAsync();

            Thread.Sleep(10000);

            // Create server
            var wss = new WebSocketServer(LOG, 8844);
            wss.AddWebSocketService<WebSocketTestBehaviour>("/test");
            wss.Start();

            Thread.Sleep(6000);

            wsc.Send("BALUS");

            gate.AssertWaitFor(CheckPoint.ClientReceived, 5000);

            wss.Stop();
            Thread.Sleep(10000);

            // Create a second server
            var wss2 = new WebSocketServer(LOG, 8844);
            wss2.AddWebSocketService<WebSocketTestBehaviour>("/test");
            wss2.Start();

            Thread.Sleep(6000);

            wsc.Send("New server, old client");

            gate.AssertWaitFor(CheckPoint.ClientReceived, 5000);

            Assert.Equal(2, clientMessageCount);

            wsc.CloseTaskAsync(CloseStatusCode.Normal, "Finished Tests");

            gate.AssertWaitFor(CheckPoint.ClientConnectionClosed, 50000);
            Assert.Equal(1, clientCloseCount);

            // Now close the second sever
            wss2.Stop();
        }
    }

    public class WebSocketTestBehaviour : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "BALUS"
                ? "I've been balused already..."
                : "I'm not available now.";

            Send(msg);
        }
    }
}
