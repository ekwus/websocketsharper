using System;
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
            ClientReceived
        }

        [Fact]
        public void TestConnect()
        {
            var gate = new TestGate<CheckPoint>();
            var wss = new WebSocketServer(LOG, 8844);
            wss.AddWebSocketService<WebSocketTestBehaviour>("/test");
            wss.Start();


            using (var wsc = new WebSocket(LOG, "ws://localhost:8844/test"))
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
